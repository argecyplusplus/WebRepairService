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

    // Добавляем новое свойство только для отображения
    [Display(Name = "Время выполнения")]

    //вариант с полными словами
    public string FormattedWorktimeFullWords
    {
        get
        {
            string hoursWord = MinimalWorktime.Hours switch
            {
                1 => "час",
                2 or 3 or 4 => "часа",
                _ => "часов"
            };

            string minutesWord = MinimalWorktime.Minutes switch
            {
                1 => "минута",
                2 or 3 or 4 => "минуты",
                _ => "минут"
            };

            if (MinimalWorktime.Hours == 0)
            {
                return $"{MinimalWorktime.Minutes} {minutesWord}";
            }
            else if (MinimalWorktime.Minutes == 0)
            {
                return $"{MinimalWorktime.Hours} {hoursWord}";
            }
            else
            {
                return $"{MinimalWorktime.Hours} {hoursWord} {MinimalWorktime.Minutes} {minutesWord}";
            }
        }
    }
}