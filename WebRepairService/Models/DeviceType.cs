﻿public class DeviceType
{
    public int DeviceTypeId { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
