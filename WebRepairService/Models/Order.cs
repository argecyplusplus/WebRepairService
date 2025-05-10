public class Order
{
    public int OrderId { get; set; }
    public int StatusId { get; set; } = 1;

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; }

    public string Model { get; set; } = string.Empty;
    public int DeviceTypeId { get; set; }
    public string Details { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0m;

    public string ClientFullName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;

    public int ServiceTypeId { get; set; }

    public string OperatorId { get; set; } = string.Empty;
    public string? EngineerId { get; set; }

    public virtual ApplicationUser Operator { get; set; } = null!;
    public virtual ApplicationUser? Engineer { get; set; }
    public virtual Status Status { get; set; } = null!;
    public virtual DeviceType DeviceType { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}