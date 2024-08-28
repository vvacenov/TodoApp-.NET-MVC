using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models.Auth
{
    public class RegistrationViewModel
    {

        [Required(ErrorMessage = UsernameRequiredMessage)]
        [StringLength(50, ErrorMessage = UsernameLengthMessage)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = PasswordRequiredMessage)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = PasswordNotValidMessage)]
        [RegularExpression(PasswordRegex, ErrorMessage = PasswordNotValidMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        private const string UsernameRequiredMessage = "Please enter a username.";
        private const string UsernameLengthMessage = "Please, choose a username between 6 - 50 characters in length.";
        private const string PasswordRequiredMessage = "Please enter a password.";
        private const string PasswordNotValidMessage = "The password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, and one number.";
        private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
    }
}
