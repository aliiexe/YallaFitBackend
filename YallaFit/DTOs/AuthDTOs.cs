namespace YallaFit.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string NomComplet { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public string Role { get; set; } = "Sportif";
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string NomComplet { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
