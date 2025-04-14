public class Order
{
    public int IdOrder { get; set; }
    public int StatusId { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow; // или другое значение по умолчанию
    public DateTime CompletionDate { get; set; } = DateTime.UtcNow; // или другое значение по умолчанию
    public string Model { get; set; } = string.Empty;
    public int DeviceTypeId { get; set; }
    public string Details { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0m; // или другое значение по умолчанию
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public virtual ApplicationUser User { get; set; } = new ApplicationUser();
    public virtual Status Status { get; set; } = new Status();
    public virtual DeviceType DeviceType { get; set; } = new DeviceType();
    public virtual ServiceType ServiceType { get; set; } = new ServiceType();
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}