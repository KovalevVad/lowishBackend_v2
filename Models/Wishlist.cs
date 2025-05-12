using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lowishBackend_v2.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // Дата создания
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Внешний ключ для связи с пользователем
        public int UserId { get; set; }

        // Навигационное свойство для User
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Навигационное свойство для подарков в вишлисте
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
    }
}