using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationMVC.Models;

//пришлось именовать маленькими потому что после пререименования на инглиш, посгре почему то начал ругаться на то
//что он не понимает верхний регистр, вроде с версии 12.5 такая ошибка появилась
public class Objects
    {
        [Key]
        public int id { get; set; }
        public string type { get; set; } // 
        public string product { get; set; } // 
    }

public class Links
{
    [Key]
    public int id { get; set; }
    public int? parentId { get; set; }
    public int childId { get; set; } // 
}

public class Attributes
{
    [Key]
    public int id { get; set; }
    public string name { get; set; } // 
    public string value { get; set; }
}




public class United
{
    public Objects ObjFromUnited { get; set; }
    public Attributes AttrFromUnited { get; set; }
    public Links LinksFromUnited { get; set; }
}

public class ListUnited
{
    public int? Seed { get; set; } = 0; //Корневой элемент

    public int NowElement { get; set; } 
    public List<United> LUnited { get; set; }
}