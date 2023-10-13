using System.ComponentModel.DataAnnotations;

namespace View.Models
{
    public class LoginObject
    {
        [Required(ErrorMessage = "username is required")]
        public string username { get; set; }
        [Required(ErrorMessage = "password is required")]
        public string password { get; set; }
    }
}
