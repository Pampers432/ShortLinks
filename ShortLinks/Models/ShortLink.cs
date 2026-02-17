using System.ComponentModel.DataAnnotations;

namespace ShortLinks.Models
{
    public class ShortLink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите URL")]
        [MaxLength(2048)]
        [Url(ErrorMessage = "Введите корректный URL (http или https)")]
        public string LongUrl { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int ClickCount { get; set; }
    }
}