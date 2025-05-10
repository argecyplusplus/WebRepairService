using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebRepairService.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Поле ФИО обязательно")]
        [StringLength(100, ErrorMessage = "ФИО не должно превышать 100 символов")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Поле Email обязательно")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Изображение профиля")]
        public IFormFile? ProfileImage { get; set; }

        [HiddenInput]
        public string? CurrentImagePath { get; set; }
    }
}
