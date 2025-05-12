using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lowishBackend_v2.Models
{
    public class Gift
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Url]
        public string? ProductUrl { get; set; }

        // Дата добавления
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Внешний ключ для связи с вишлистом
        public int WishlistId { get; set; }

        // Навигационное свойство для вишлиста
        [ForeignKey("WishlistId")]
        public Wishlist Wishlist { get; set; } = null!;
    }
}