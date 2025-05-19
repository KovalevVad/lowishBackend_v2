using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lowishBackend_v2.Models
{
    public class Gift
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // URL изображения подарка (опционально)
        public string? ImageUrl { get; set; }

        // Предполагаемая цена (опционально)
        public decimal? Price { get; set; }

        // URL для покупки подарка (опционально)
        public string? PurchaseUrl { get; set; }

        // Статус подарка (например, "Желаемый", "Купленный", "Зарезервированный")
        public string? Status { get; set; }

        // Внешний ключ для связи с вишлистом
        public int WishListId { get; set; }

        // Навигационное свойство для связи с вишлистом
        [ForeignKey("WishListId")]
        [JsonIgnore]
        public WishList WishList { get; set; } = null!;

        // Дата добавления подарка
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}