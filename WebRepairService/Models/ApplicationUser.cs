using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = null!;

    [Display(Name = "Дата регистрации")]
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public string? ProfileImage { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}