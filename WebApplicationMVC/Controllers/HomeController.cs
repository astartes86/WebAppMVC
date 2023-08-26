using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

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

        public ActionResult FolderFileTree()//дерево
        {
            List<Объединенная> combinedTables = new List<Объединенная>();
            var connection = new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Database=Проводник5;Username=postgres;Password=Coraks_86_");
            connection.Open();
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.КодПапки, Папки.Название, Папки.КодРодительскойПапки, " +
                                                            "Файлы.Название, Файлы.КодФайла, Расширения.Иконка " +
                                                            "FROM Папки LEFT JOIN Файлы ON Папки.КодПапки = Файлы.КодПапки " +
                                                                       "LEFT JOIN Расширения ON Файлы.КодТипаФайла = Расширения.КодТипаФайла " +
                                                            "ORDER BY Папки.Название, Папки.КодПапки, Файлы.Название, Файлы.КодФайла", connection);
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
                            //при добавлении папки которая не содержит файл
                            Файл table2 = new()
                            {
                                Название = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                КодФайла = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
                                // Set other properties
                            };
                            Расширение table3 = new()
                            {
                                Иконка = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                // Set other properties
                            };
                combinedTables.Add(new Объединенная { ПапкаИзОбъединенной = table1, ФайлИзОбъединенной = table2, РасширениеИзОбъединенной = table3 });
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
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.КодПапки, Папки.Название, Папки.КодРодительскойПапки, " +
                                                            "Файлы.Название, Файлы.КодФайла FROM Папки LEFT JOIN Файлы ON Папки.КодПапки = Файлы.КодПапки " +
                                                            "ORDER BY Папки.Название, Папки.КодПапки, Файлы.Название, Файлы.КодФайла", connection);
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
                //при добавлении папки которая не содержит файл
                //string названиевременое;
                //int кодФайлавременое;
                //if (reader.IsDBNull(3))
                //    названиевременое = "";
                //else названиевременое = reader.GetString(3);
                //if (reader.IsDBNull(4))
                //    кодФайлавременое = -1;
                //else кодФайлавременое = reader.GetInt32(4);

                Файл table2 = new()
                {
                    Название = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    КодФайла = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
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

        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> FileContent(int id)
        {
            return View(await db.Файлы.Where(s => s.КодФайла == id).ToListAsync());
        }
        //--------------------------------------------------------------------------------------------------
        //public async Task<IActionResult> Icone(int id)
        //{
        //    return View(await db.Расширения.Where(s => s.КодТипаФайла == id).ToListAsync());
        //}
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Icone()//тестил иконку - потом удалить
        {
            return View(await db.Расширения.Where(s => s.КодТипаФайла == 1).ToListAsync());
        }

//--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Папка папка)
        {
            db.Папки.Add(папка);
            await db.SaveChangesAsync();
            return RedirectToAction("FolderFileTree");
        }

//--------------------------------------------------------------------------------------------------
                [HttpPost]
                public async Task<IActionResult> DeleteFolder(Папка папка)
                {
                    if (папка.КодПапки != null)
                    {
                        db.Entry(папка).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        return RedirectToAction("FolderFileTree");
                    }
                    return NotFound();
                    //return $"{папка.КодПапки}";
        }
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateFolder(Объединенная сущность) //из формы на странице Rename,
                                                                             //если есть все данные в форме то редактирует запись,
                                                                             //если нет - то добавляет новую
        {
                db.Папки.Update(сущность.ПапкаИзОбъединенной);
                await db.SaveChangesAsync();
                return RedirectToAction("FolderFileTree");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFile(Объединенная сущность)  //из формы на странице Rename,
                                                                            //если есть все данные в форме то редактирует запись,
                                                                            //если нет - то добавляет новую
        {
            db.Файлы.Update(сущность.ФайлИзОбъединенной);
            await db.SaveChangesAsync();
            return RedirectToAction("FolderFileTree");
        }
        [HttpPost]
        public async Task<IActionResult> Rename(Объединенная сущность)
        {
            if (сущность.ПапкаИзОбъединенной != null)
                { сущность.ПапкаИзОбъединенной = await db.Папки.FirstOrDefaultAsync(p => p.КодПапки == сущность.ПапкаИзОбъединенной.КодПапки); }
            if (сущность.ФайлИзОбъединенной != null)
                { сущность.ФайлИзОбъединенной = await db.Файлы.FirstOrDefaultAsync(p => p.КодФайла == сущность.ФайлИзОбъединенной.КодФайла);}
            return View(сущность);
        }
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Rename(int? id)
        {
            if (id != null)
            {
                Объединенная сущность = new()
                { ПапкаИзОбъединенной = await db.Папки.FirstOrDefaultAsync(p => p.КодПапки == id),
                  ФайлИзОбъединенной = await db.Файлы.FirstOrDefaultAsync(p => p.КодФайла == id) };
                if (сущность.ПапкаИзОбъединенной != null) return View(сущность);
                if (сущность.ФайлИзОбъединенной != null) return View(сущность);
            }
            return NotFound();
        }

        //--------------------------------------------------------------------------------------------------
        public IActionResult DeleteFile()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------
        public IActionResult DeleteFolder()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------
        public IActionResult LoadFile()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------
        public IActionResult GetFile()
        {
            return View();
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
                public async Task<IActionResult> DeleteFile(Файл файл)
                {
                    if (файл.КодФайла!= null)
                    {
                        db.Entry(файл).State = EntityState.Deleted;
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