public class ServiceType
{
    public int ServiceTypeId { get; set; }
    public string Name { get; set; } = null!;
    public decimal MinimalPrice { get; set; }
    public TimeSpan MinimalWorktime { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
