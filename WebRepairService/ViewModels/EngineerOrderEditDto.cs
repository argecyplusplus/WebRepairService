using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebRepairService.Models
{
    public class EngineerOrderEditDto
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public int StatusId { get; set; }

        public string? EngineerId { get; set; }

        // Только для отображения
        public string? CurrentStatus { get; set; }
        public string? CurrentEngineer { get; set; }

        // Списки для выпадающих меню
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Engineers { get; set; } = new();
    }
}