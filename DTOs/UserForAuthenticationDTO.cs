using System.ComponentModel.DataAnnotations;

namespace BookLib.DTOs
{
    public class UserForAuthenticationDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; init; }
        [Required(ErrorMessage = "Passowrd is required")]
        public string? Password { get; init; }
    }
}
