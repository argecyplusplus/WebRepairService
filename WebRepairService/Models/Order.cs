public class Order
{
    public int IdOrder { get; set; }
    public int StatusId { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; } // Nullable, если заказ не завершен

    public string Model { get; set; } = string.Empty;
    public int DeviceTypeId { get; set; }
    public string Details { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0m;

    public string ClientFullName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;

    public int ServiceTypeId { get; set; }

    // Измененные пользовательские связи
    public string OperatorId { get; set; } = string.Empty; // Кто принял заказ
    public string EngineerId { get; set; } = string.Empty; // Кто выполняет

    public virtual ApplicationUser Operator { get; set; } = null!;
    public virtual ApplicationUser Engineer { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
    public virtual DeviceType DeviceType { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}