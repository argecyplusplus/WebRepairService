using System.ComponentModel.DataAnnotations;

public class ServiceType
{
    public int ServiceTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "Standard Service"; // Not null с дефолтным значением

    public decimal MinimalPrice { get; set; } = 0; // Дефолтное значение

    public TimeSpan MinimalWorktime { get; set; } = TimeSpan.FromHours(1); // Дефолтное значение

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}