using System.ComponentModel.DataAnnotations;

public class ServiceTypeViewModel
{
    public int ServiceTypeId { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [Display(Name = "Название услуги")]
    [MaxLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Минимальная цена обязательна")]
    [Display(Name = "Минимальная цена")]
    [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]
    public decimal MinimalPrice { get; set; }

    [Required(ErrorMessage = "Минимальное время обязательно")]
    [Display(Name = "Минимальное время работы")]
    public TimeSpan MinimalWorktime { get; set; }
}