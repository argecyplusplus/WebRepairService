using System.ComponentModel.DataAnnotations;

public class ServiceType
{
    public int ServiceTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "Standard Service";

    public decimal MinimalPrice { get; set; } = 0;

    public TimeSpan MinimalWorktime { get; set; } = TimeSpan.FromHours(1);

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}