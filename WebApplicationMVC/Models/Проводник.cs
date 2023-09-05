using System.ComponentModel.DataAnnotations;
namespace WebApplicationMVC.Models;

//пришлось именовать маленькими потому что после пререименования на инглиш, посгре почему то начал ругаться на то
//что он не понимает верхний регистр, вроде с версии 12.5 такая ошибка появилась
public class Folder
    {
        [Key]
        public int codefolder { get; set; }
        public string? name { get; set; } // 
        public int codeparentfolder { get; set; } // 
    }

public class File
{
    [Key]
    public int codefile { get; set; }
    public string? namefile { get; set; } // 
    public string? about { get; set; }
    public int codetypefolder { get; set; } // 
   //[Key]
    public int codefolder { get; set; }
    public string? Content { get; set; }
}
public class Extension
{
    [Key]
    public int codetypefolder { get; set; }
    public string? type { get; set; } // 
    public string? icone { get; set; } // 
}
public class ПапкаПохожая
{
    // [Key]
    internal ПапкаПохожая(Folder папка)
    {
        this.КодПапки = папка.codefolder;
        this.Название = папка.name;
        this.КодРодительскойПапки = папка.codeparentfolder;
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




public class ПапкиФайлыРасш
{
    // [Key]
    internal ПапкиФайлыРасш(Folder папка)
    {
        this.КодПапки = папка.codefolder;
        this.Название = папка.name;
        this.КодРодительскойПапки = папка.codeparentfolder;
        //this.НазваниеФайла = файл.Название;
        //this.КодФайла = файл.КодФайла;
    }
    public int КодПапки { get; set; }
    public string? Название { get; set; } // 
    public int КодРодительскойПапки { get; set; } // 
    public string? НазваниеФайла { get; set; } // 
    public int КодФайла { get; set; } // 
}

public class СписокПапкиФайлыРасш
{ 
    public int? Seed { get; set; } = 0; //Корневой элемент
    public IEnumerable<ПапкиФайлыРасш> СПапкиФайлыРасш { get; set; } 
}


public class Объединенная
{
    public Folder ПапкаИзОбъединенной { get; set; }
    public File ФайлИзОбъединенной { get; set; }
    public Extension РасширениеИзОбъединенной { get; set; }
}

public class СписокОбъединенная
{
    public int? Seed { get; set; } = 0; //Корневой элемент

    public int? КодТекущаяПапка { get; set; } 
    public List<Объединенная>? CОбъединенная { get; set; }
}