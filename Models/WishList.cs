using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lowishBackend_v2.Models
{
    public class WishList
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        // Внешний ключ для связи с пользователем
        public int UserId { get; set; }

        // Навигационное свойство для связи с пользователем
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Дата создания вишлиста
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}