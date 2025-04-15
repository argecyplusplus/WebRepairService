using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class OrderViewModel
{
    public int IdOrder { get; set; }

    // Информация о пользователях
    [Required(ErrorMessage = "Необходимо указать оператора")]
    public string OperatorId { get; set; } = string.Empty;

    public string? EngineerId { get; set; } // Может быть null, если еще не назначен

    public UserViewModel? Operator { get; set; }
    public UserViewModel? Engineer { get; set; }


    [Required(ErrorMessage = "Статус заказа обязателен")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "Дата создания обязательна")]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Дата завершения обязательна")]
    public DateTime CompletionDate { get; set; } = DateTime.UtcNow.AddDays(7); // Пример: +7 дней по умолчанию

    [Required(ErrorMessage = "Модель устройства обязательна")]
    [StringLength(100, ErrorMessage = "Модель не должна превышать 100 символов")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Тип устройства обязателен")]
    public int DeviceTypeId { get; set; }

    [Required(ErrorMessage = "Описание обязательно")]
    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string Details { get; set; } = string.Empty;

    [Required(ErrorMessage = "Цена обязательна")]
    [Range(0, 1000000, ErrorMessage = "Цена должна быть между 0 и 1 000 000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "ФИО клиента обязательно")]
    [StringLength(150, ErrorMessage = "ФИО не должно превышать 150 символов")]
    public string ClientFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон клиента обязателен")]
    [Phone(ErrorMessage = "Неверный формат телефона")]
    [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
    public string ClientPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email клиента обязателен")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
    public string ClientEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Тип услуги обязателен")]
    public int ServiceTypeId { get; set; }

    // Опционально: можно включить связанные данные, если они нужны во View
    public UserViewModel? User { get; set; }
    public StatusViewModel? Status { get; set; }
    public DeviceTypeViewModel? DeviceType { get; set; }
    public ServiceTypeViewModel? ServiceType { get; set; }
    public List<PhotoViewModel> Photos { get; set; } = new List<PhotoViewModel>();
}