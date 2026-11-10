using System.ComponentModel.DataAnnotations;
using DndForum.Models;

namespace DndForum.Dtos;

public class RegisterRequest
{
    private const int UserNameMinLength = 3;
    private const int PasswordMinLength = 8;
    private const int PasswordMaxLength = 128;

    [Required]
    [StringLength(User.UserNameMaxLength, MinimumLength = UserNameMinLength)]
    [RegularExpression("^[a-zA-Z0-9_.-]+$", ErrorMessage = "Vartotojo vardą gali sudaryti tik lotyniškos raidės, skaičiai ir simboliai _ . -")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(User.EmailMaxLength)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Slaptažodyje turi būti bent viena raidė ir bent vienas skaičius.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AuthResponse(string AccessToken, string TokenType, DateTime ExpiresAt, UserResponse User);
