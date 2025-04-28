using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class OrderViewModel
{
    public int OrderId { get; set; }

    [Required(ErrorMessage = "ФИО клиента обязательно")]
    public string ClientFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон клиента обязателен")]
    public string ClientPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email клиента обязателен")]
    [EmailAddress]
    public string ClientEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Модель устройства обязательна")]
    public string DeviceModel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Details { get; set; } = string.Empty;

    [Required(ErrorMessage = "Цена обязательна")]
    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Тип устройства обязателен")]
    public int DeviceTypeId { get; set; }

    [Required(ErrorMessage = "Тип услуги обязателен")]
    public int ServiceTypeId { get; set; }

    public string? EngineerId { get; set; }
    public List<SelectListItem> DeviceTypes { get; set; } = new();
    public List<SelectListItem> ServiceTypes { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();
    public List<SelectListItem> Engineers { get; set; } = new();
    public List<IFormFile>? Photos { get; set; }
}