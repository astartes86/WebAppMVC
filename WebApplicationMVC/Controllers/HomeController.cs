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
        public IActionResult Client(Client client)//в индексе форма настрона на стр клиент. обрабатываем метод_
                                                   //пост отправляемый при нажатии на кнопку сабмит в стр клиент, он сам передается в параметр 
        {
            MyClients myclibase = new();
            //Client cli = new Client { Procent = client.Procent, Summa = client.Summa, Long = client.Long, Platej = client.Procent / 100 / 12 * client.Summa };
            double k = client.Stavka / 100 / 12 * Math.Pow(1 + client.Stavka / 100 / 12, client.Long);
            double low = Math.Pow(1 + client.Stavka / 100 / 12, client.Long) - 1;
            k = k / low;
            client.TotalPayment = k * client.Summa;
            double Sn = client.Summa;
            double Stavka = client.Stavka;
            double TotalPayment = client.TotalPayment;
            DateTime currdate = DateTime.Now;
            for (int i = 0; i < (int)client.Long; i++)
            {
                double ProcentPayment = Sn * Stavka / 100 / 12;
                double MainPayment = TotalPayment - ProcentPayment;
                Sn -= MainPayment;
                currdate=currdate.AddMonths(1);
                Client tempclient = new();
                tempclient.Summa = Sn;//теперь это остаток долга
                tempclient.CreatedDate = currdate;
                tempclient.ProcentPayment = ProcentPayment;
                tempclient.MainPayment = MainPayment;
                tempclient.Stavka = Stavka;
                tempclient.Long = i+1;
                tempclient.TotalPayment = TotalPayment;
                myclibase.Clients.Add(tempclient);//происходит добавление последнего элемента во все существующие, _
                                                  //надо создать новый экземпляр элемента внутри цикла для каждой итерации.
            }
            return View(myclibase);
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

    //public record class MyClient(string Name);
    
}