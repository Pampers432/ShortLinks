using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public LinksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var links = await _context.ShortLinks
                .OrderByDescending(x => x.CreatedAt)
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

            model.Code = await GenerateUniqueCodeAsync();
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
            var link = await _context.ShortLinks
                .FirstOrDefaultAsync(x => x.Code == code);

            if (link == null)
                return NotFound();

            link.ClickCount++;
            await _context.SaveChangesAsync();

            return Redirect(link.LongUrl);
        }

        private async Task<string> GenerateUniqueCodeAsync(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            while (true)
            {
                var bytes = RandomNumberGenerator.GetBytes(length);
                var code = new StringBuilder(length);

                foreach (var b in bytes)
                {
                    code.Append(chars[b % chars.Length]);
                }

                string finalCode = code.ToString();

                bool exists = await _context.ShortLinks
                    .AnyAsync(x => x.Code == finalCode);

                if (!exists)
                    return finalCode;
            }
        }

        private bool IsValidHttpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
