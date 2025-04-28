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

        public string? CurrentStatus { get; set; }
        public string? CurrentEngineer { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Engineers { get; set; } = new();
    }
}