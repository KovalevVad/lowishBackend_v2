namespace lowishBackend_v2.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Message { get; set; }
    }
}