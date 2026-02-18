using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShortLinks.Data;
using ShortLinks.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ShortLinks.Controllers
{
    public class LinksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public LinksController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var links = await _context.ShortLinks
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(links);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShortLink model)
        {
            if (!IsValidHttpUrl(model.LongUrl))
            {
                ModelState.AddModelError("LongUrl", "Разрешены только http и https ссылки");
            }

            if (!ModelState.IsValid)
                return View(model);

            model.Code = GenerateCode();

            // Проверяем коллизию,
            // т.к. теоретически одинаковый код может быть сгенерирован повторно.
            while (await _context.ShortLinks.AnyAsync(x => x.Code == model.Code))
            {
                model.Code = GenerateCode();
            }

            model.CreatedAt = DateTime.UtcNow;
            model.ClickCount = 0;

            _context.ShortLinks.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var link = await _context.ShortLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShortLink model)
        {
            if (id != model.Id)
                return NotFound();

            if (!IsValidHttpUrl(model.LongUrl))
            {
                ModelState.AddModelError("LongUrl", "Разрешены только http и https ссылки");
            }

            if (!ModelState.IsValid)
                return View(model);

            var link = await _context.ShortLinks.FindAsync(id);
            if (link == null)
                return NotFound();

            link.LongUrl = model.LongUrl;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var link = await _context.ShortLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var link = await _context.ShortLinks.FindAsync(id);

            if (link != null)
            {
                _context.ShortLinks.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RedirectToLongUrl(string code)
        {
            if (!_cache.TryGetValue(code, out string originalUrl))
            {
                var link = await _context.ShortLinks
                    .AsNoTracking() // Не отслеживаем, т.к. только читаем
                    .FirstOrDefaultAsync(x => x.Code == code);

                if (link == null)
                    return NotFound();

                originalUrl = link.LongUrl;

                _cache.Set(code, originalUrl,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
            }

            await IncrementClickCount(code);

            return Redirect(originalUrl);
        }

        //не загружается объект, нет трекинга EF, один SQL-запрос
        private async Task IncrementClickCount(string shortCode)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE ShortLinks SET ClickCount = ClickCount + 1 WHERE Code = {0}",
                shortCode);
        }

        private string GenerateCode()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            // Даёт короткий, контролируемый и равномерно случайный код,
            // что отлично подходит для тестового решения
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool IsValidHttpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
