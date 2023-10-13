using System.ComponentModel.DataAnnotations;

namespace View.Models
{
    public class CreateNewUser
    {
        [Required(ErrorMessage = "Name is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "You need to add an password")]
        public string password { get; set; }

        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; }
    }
}
