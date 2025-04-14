public class Photo
{
    public int PhotoId { get; set; }
    public string Link { get; set; } = null!;
    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
