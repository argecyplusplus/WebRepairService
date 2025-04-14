public class Status
{
    public int StatusId { get; set; }
    public string Name { get; set; } = string.Empty; // Not null, default empty
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}