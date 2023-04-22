using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class Client
    {
        //[HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        [Display(Name = "Название")]
        [StringLength(10, MinimumLength =5, ErrorMessage ="Длина строки менее 5")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Надо указать сумму")]
        [Range(1000,1000000, ErrorMessage = "Не верная сумма: 1000-1млн")]
        public double Summa { get; set; }
        [Required(ErrorMessage = "Надо указать срок")]
        [Range(6, 36, ErrorMessage = "Не верный срок: 6-36")]
        public double Long { get; set; } //= 15;
        [Required(ErrorMessage = "Надо указать ставку")]
        [Range(0, 50, ErrorMessage = "Не верный срок: 0-50")]
        public double Stavka { get; set; }
        public double MainPayment { get; set; }
        public double ProcentPayment { get; set; }
        public double TotalPayment { get; set; }
        public DateTime CreatedDate { get; set; } 
        //public string Description { get; set; }
    }
    public class MyClients
    {
        public List<Client> Clients { get; set; } = new List<Client>();
    }
}
