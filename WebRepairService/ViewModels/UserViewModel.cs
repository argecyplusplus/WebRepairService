using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public string Id { get; set; }

    [Required]
    [Display(Name = "ФИО")]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Роли")]
    public List<RoleViewModel> Roles { get; set; } = new();
}

public class RoleViewModel
{
    public string Name { get; set; }
    public bool IsSelected { get; set; }
}