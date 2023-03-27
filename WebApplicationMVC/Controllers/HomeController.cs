using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //ViewBag.People = new List<string> { "Tom", "Sam", "Bob" };
            //var people = new List<string> { "Tom", "Sam", "Bob" };
            var client = new Client { Id = 222, Name = "vladimir" };//создаем клиента
            return View(client);
        }
        //[HttpPost]
        //public string Index(string username) => $"User Name: {username}";
        //[HttpPost]
        //public string Index(int summ, int time, int procent, string comment)
        //{
        //    return $"Summ: {summ}   time: {time}   procent: {procent}  Comment: {comment}";
        //}

        //[HttpPost]
        //public IActionResult Client(Client client)
        //public IActionResult Client()
        //{
        //string authData = $"Имя: {client.Name}   Сумма: {client.Summa}   Срок: {client.Long}  Процент: {client.Procent}";
        //return Content($"{authData}");
        //return View();
        //}
        //[HttpPost]
        //public IActionResult Client(int Summa, int Long, int Procent, string Name)
        //public IActionResult Client(string Name)
        //{
            //string authData = $"Имя: {Name}   Сумма: {Summa}   Срок: {Long}  Процент: {Procent}";
            //return Content($"{Name}");
        //}

        [HttpGet]
        public IActionResult Client() => View();
        [HttpPost]
        public string Client(int Summa, int Long, int Procent, string Name) => $"Имя: {Name}   Сумма: {Summa}   Срок: {Long}  Процент: {Procent}";

        //public IActionResult Client()
        //{ ViewBag.Message = "asdfsdf";
        //        return View();}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}