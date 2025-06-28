using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MusicStreamer.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Primeiro Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;


    }

}
