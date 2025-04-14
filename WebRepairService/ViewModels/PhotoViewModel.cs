using System.ComponentModel.DataAnnotations;

public class PhotoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ссылка на фото обязательна")]
    [Url(ErrorMessage = "Некорректный URL изображения")]
    [MaxLength(500, ErrorMessage = "Ссылка не должна превышать 500 символов")]
    [Display(Name = "Ссылка на фото")]
    public string Link { get; set; }

    [Required(ErrorMessage = "ID заказа обязателен")]
    [Display(Name = "ID заказа")]
    public int OrderId { get; set; }

}