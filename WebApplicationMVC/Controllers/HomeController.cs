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
            var people = new List<string> { "Tom", "Sam", "Bob" };
            var client = new Client { Id = 222, Name = "vladimir"
        };//создаем клиента
            return View(client);
        }
        //[HttpPost]
        //public string Index(string username) => $"User Name: {username}";
        [HttpPost]
        public string Index(string username, string password, int age, string comment)
        {
            return $"User Name: {username}   Password: {password}   Age: {age}  Comment: {comment}";
        }
        [HttpPost]
        public IActionResult Client(string login, string password, int age, string comment)
        {
            string authData = $"Login: {login}   Password: {password}   Age: {age}  Comment: {comment}";
            return Content(authData);
        }
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