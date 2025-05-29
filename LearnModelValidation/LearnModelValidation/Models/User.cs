using MinimalApis.Extensions.Filters;
using System.ComponentModel.DataAnnotations;

namespace LearnModelValidation.Models
{
    public   abstract class User
    {
        protected User()
        {
        }

        protected User(string email, string password)
        {
            Email = email;
            Password = password;
        }
        [Required]
        [EmailAddress]
        public string Email { get;  set; }
        [Required]
        [MinLength(6)]
        public string Password { get;  set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmedPassword { get; set; } = string.Empty;


    }
}
