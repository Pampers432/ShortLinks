using System.ComponentModel.DataAnnotations;

namespace ShortLinks.Models
{
    public class ShortLink
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(2048)]
        public string LongUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int ClickCount { get; set; }
    }
}