using System.ComponentModel.DataAnnotations;

public class Photo
{
    public int PhotoId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Link { get; set; } = string.Empty; // Заменяем null! на string.Empty

    [Required]
    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!; // Навигационное свойство для EF
}