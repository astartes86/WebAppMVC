using System.ComponentModel.DataAnnotations;
namespace WebApplicationMVC.Models;
public class Папка
    {
        [Key]
        public int КодПапки { get; set; }
        public string? Название { get; set; } // 
        public string? КодРодительскойПапки { get; set; } // 
    }
