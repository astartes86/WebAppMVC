using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApplicationMVC.Controllers
{
    public class HomeController : Controller
    {

        ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
//--------------------------------------------------------------------------------------------------
        public ActionResult Folder()
        {
            //using (ApplicationContext context = new ApplicationContext())

                СписокПапок model = new()
                {
                    СПапки = db.Папки.ToArray().Select(x => new ПапкаПохожая(x) )
                                                                //{ Console.WriteLine("index id=" + x.Название); return new ПапкаПохожая(x);} надо запомнить!)
                };
                return View(model);

        }

        //--------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult FolderFile2()//нерабочая версия
        {
            var FFE = from Папка in db.Папки
                      join Файл in db.Файлы on Папка.КодПапки equals Файл.КодПапки
                      //toDO//join Расширение in db.Расширения on Файл.КодТипаФайла equals Расширение.КодТипаФайла
                      select new
                      {
                          КодПапки = Папка.КодПапки,
                          Название = Папка.Название,
                          КодРодительскойПапки = Папка.КодРодительскойПапки,
                          НазваниеФайла = Файл.Название,
                          КодФайла = Файл.КодФайла
                      };

            foreach (var u in FFE)
            Console.WriteLine($"{u.КодПапки} - {u.Название} - {u.КодРодительскойПапки} - {u.НазваниеФайла} - {u.КодФайла}");
            return View();
        }

        public ActionResult FolderFile3()//дерево
        {
            List<Объединенная> combinedTables = new List<Объединенная>();
            var connection = new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Database=Проводник5;Username=postgres;Password=Coraks_86_");
            connection.Open();
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.КодПапки, Папки.Название, Папки.КодРодительскойПапки, Файлы.Название, Файлы.КодПапки FROM Папки LEFT JOIN Файлы ON Папки.КодПапки = Файлы.КодПапки", connection);
                var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var table1 = new Папка
                            {
                                КодПапки = reader.GetInt32(0),
                                Название = reader.GetString(1),
                                КодРодительскойПапки = reader.GetInt32(2),
                                // Set other properties
                            };
                            var table2 = new Файл
                            {
                                Название = reader.GetString(3),
                                КодФайла = reader.GetInt32(4),
                                // Set other properties
                            };
                            combinedTables.Add(new Объединенная { ПапкаИзОбъединенной = table1, ФайлИзОбъединенной = table2 });
                        }
            СписокОбъединенная model = new() { CОбъединенная = combinedTables };
            
            return View(model);//для вывода в виде дерева
        }

//--------------------------------------------------------------------------------------------------
        public ActionResult FolderFile()//таблица
        {
            List<Объединенная> combinedTables = new List<Объединенная>();
            var connection = new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Database=Проводник5;Username=postgres;Password=Coraks_86_");
            connection.Open();
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.КодПапки, Папки.Название, Папки.КодРодительскойПапки, Файлы.Название, Файлы.КодПапки FROM Папки LEFT JOIN Файлы ON Папки.КодПапки = Файлы.КодПапки", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Папка table1 = new Папка
                {
                    КодПапки = reader.GetInt32(0),
                    Название = reader.GetString(1),
                    КодРодительскойПапки = reader.GetInt32(2),
                    // Set other properties
                };
                Файл table2 = new Файл
                {
                    Название = reader.GetString(3),
                    КодФайла = reader.GetInt32(4),
                    // Set other properties
                };
                combinedTables.Add(new Объединенная { ПапкаИзОбъединенной = table1, ФайлИзОбъединенной = table2 });
            }
            return View(combinedTables);//для вывода в виде таблицы
        }
//--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            return View(await db.Папки.ToListAsync());
        }
//--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> File()
        {
            return View(await db.Файлы.ToListAsync());
        }
        //--------------------------------------------------------------------------------------------------

        public IActionResult Index2()
        {
                return View();
        }
//--------------------------------------------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }
//--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Папка папка)
        {
            db.Папки.Add(папка);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
//--------------------------------------------------------------------------------------------------
        public IActionResult DeleteF()
        {
            return View();
        }
//--------------------------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> DeleteF(Папка папка)
        {
            if (папка.КодПапки != null)
            {
                db.Entry(папка).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
//--------------------------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult Client(Client client)//в индексе форма настрона на стр клиент. обрабатываем метод_
                                                   //пост отправляемый при нажатии на кнопку сабмит в стр клиент, он сам передается в параметр 
        {
            if (ModelState.IsValid)//валидация на сервере
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
                    currdate = currdate.AddMonths(1);
                    Client tempclient = new();
                    tempclient.Summa = Sn;//теперь это остаток долга
                    tempclient.CreatedDate = currdate;
                    tempclient.ProcentPayment = ProcentPayment;
                    tempclient.MainPayment = MainPayment;
                    tempclient.Stavka = Stavka;
                    tempclient.Long = i + 1;
                    tempclient.TotalPayment = TotalPayment;
                    myclibase.Clients.Add(tempclient);//происходит добавление последнего элемента во все существующие, _
                                                      //надо создать новый экземпляр элемента внутри цикла для каждой итерации.
                }
                return View(myclibase);
            }
            return RedirectToAction("Index", "Home");
        }
        //public IActionResult Client2(Client client)//попробовал сгенерить форму по модели через клик в меню
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(client);
        //    }
        //    return RedirectToAction("Index", "Home"); 
        //}
//--------------------------------------------------------------------------------------------------
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