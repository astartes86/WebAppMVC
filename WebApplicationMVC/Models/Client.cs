using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class Client
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; } = "asdf";
        public double Summa { get; set; }
        public double Long { get; set; } = 15;
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
