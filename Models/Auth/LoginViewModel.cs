using System.ComponentModel.DataAnnotations;


namespace TodoApp.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = UsernameRequiredMessage)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = PasswordRequiredMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        private const string UsernameRequiredMessage = "Please enter a username.";
        private const string PasswordRequiredMessage = "Please enter a password.";
    }
}