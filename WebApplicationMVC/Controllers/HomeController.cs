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
            //var client = new Client { Id = 222, Name = "vladimir" };//создаем клиента
            return View();
        }

        [HttpGet]
        public IActionResult Client() => View();
        [HttpPost]
        public IActionResult Client(Client client)
        {
            Client cli = new Client { Procent = client.Procent, Summa = client.Summa, Long = client.Long, Platej = client.Procent / 100 / 12 * client.Summa };
            return View(cli);
        }
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
    //public record class Client(string Name);
}