public class Order
{
    public int IdOrder { get; set; }
    public int StatusId { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Model { get; set; }
    public int DeviceTypeId { get; set; }
    public string? Details { get; set; }
    public decimal? Price { get; set; }
    public string ClientFullName { get; set; } = null!;
    public string ClientPhone { get; set; } = null!;
    public string ClientEmail { get; set; } = null!;
    public int ServiceTypeId { get; set; }
    public string UserId { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
    public virtual DeviceType DeviceType { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
