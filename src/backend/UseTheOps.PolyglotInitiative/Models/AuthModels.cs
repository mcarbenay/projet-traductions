using System.ComponentModel.DataAnnotations;

namespace UseTheOps.PolyglotInitiative.Models
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ApiKeyLoginRequest
    {
        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
}
