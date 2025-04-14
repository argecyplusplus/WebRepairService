// Чистая модель данных без валидации
public class DeviceType
{
    public int DeviceTypeId { get; set; }

    public string Name { get; set; } = "Unnamed Device"; // Not null default

    // Навигационное свойство для EF Core
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}