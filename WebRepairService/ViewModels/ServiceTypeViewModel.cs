using System.ComponentModel.DataAnnotations;

public class ServiceTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название услуги обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть 2-100 символов")]
    public string Name { get; set; }

    [Range(0, 1000000, ErrorMessage = "Цена должна быть неотрицательной")]
    public decimal MinimalPrice { get; set; }

    [Range(typeof(TimeSpan), "00:00", "365.00:00", ErrorMessage = "Некорректное время работы")]
    public TimeSpan MinimalWorktime { get; set; }


}