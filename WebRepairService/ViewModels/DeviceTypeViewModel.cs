using System.ComponentModel.DataAnnotations;

public class DeviceTypeViewModel
{
    public int Id { get; set; } // Переименовано для клиента

    [Required(ErrorMessage = "Device type name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters")]
    public string Name { get; set; }

}