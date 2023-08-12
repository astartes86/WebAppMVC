using System.ComponentModel.DataAnnotations;
namespace WebApplicationMVC.Models;
public class Папка
    {
        [Key]
        public int КодПапки { get; set; }
        public string? Название { get; set; } // 
        public int КодРодительскойПапки { get; set; } // 
    }

public class Файл
{
    [Key]
    public int КодФайла { get; set; }
    public string? Название { get; set; } // 
    public string? Описание { get; set; }
    public int КодТипаФайла { get; set; } // 
   //[Key]
    public int КодПапки { get; set; }
    public string? Контент { get; set; }
}
public class Расширение
{
    [Key]
    public int КодТипаФайла { get; set; }
    public string? Тип { get; set; } // 
    public string? Иконка { get; set; } // 
}
public class ПапкаПохожая
{
    // [Key]
    internal ПапкаПохожая(Папка папка)
    {
        this.КодПапки = папка.КодПапки;
        this.Название = папка.Название;
        this.КодРодительскойПапки = папка.КодРодительскойПапки;
    }
    public int КодПапки { get; set; }
    public string? Название { get; set; } // 
    public int КодРодительскойПапки { get; set; } // 

}

public class СписокПапок
{
    public int? Seed { get; set; } = 0; //Корневой элемент
    public IEnumerable<ПапкаПохожая> СПапки { get; set; } //Список папок
}