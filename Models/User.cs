using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        // Оставим поле Name для обратной совместимости
        public string? Name { get; set; }
    }
}