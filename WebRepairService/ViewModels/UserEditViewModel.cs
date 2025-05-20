using System.ComponentModel.DataAnnotations;

namespace WebRepairService.ViewModels
{
    public class UserEditViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }
        public IList<string> Roles { get; set; }
        public string ProfileImage { get; set; }
    }
}
