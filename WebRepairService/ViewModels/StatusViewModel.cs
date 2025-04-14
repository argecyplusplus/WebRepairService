using System.ComponentModel.DataAnnotations;

public class StatusViewModel
{
    public int StatusId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}