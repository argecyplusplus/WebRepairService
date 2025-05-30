﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class OrderEditDto
{
    public int OrderId { get; set; }

    [Required(ErrorMessage = "ФИО клиента обязательно")]
    public string ClientFullName { get; set; }

    [Required(ErrorMessage = "Телефон клиента обязателен")]
    public string ClientPhone { get; set; }

    [EmailAddress(ErrorMessage = "Неверный формат email")]
    public string ClientEmail { get; set; }

    [Required(ErrorMessage = "Модель устройства обязательна")]
    public string DeviceModel { get; set; }

    public string Details { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]
    public int Price { get; set; }

    [Required(ErrorMessage = "Тип устройства обязателен")]
    public int DeviceTypeId { get; set; }

    [Required(ErrorMessage = "Тип услуги обязателен")]
    public int ServiceTypeId { get; set; }

    [Required(ErrorMessage = "Статус обязателен")]
    public int StatusId { get; set; }

    public List<IFormFile>? NewPhotos { get; set; } = new List<IFormFile>();

    public string? DeletedPhotos { get; set; } = string.Empty;

    public List<SelectListItem> DeviceTypes { get; set; } = new();
    public List<SelectListItem> ServiceTypes { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();

    public List<PhotoViewModel> Photos { get; set; } = new();
}

