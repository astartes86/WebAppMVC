using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;

namespace WebApplicationMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        ApplicationContext db;
       
        public HomeController(ApplicationContext context, IConfiguration configuration)
        {
            db = context;
            if (_configuration == null)
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                _configuration = builder.Build();
            }
            // добавляем начальные данные при их отсутствии
            if (!db.Папки.Any())
            {
                Folder root = new() { codefolder = 1, name = "Корень", codeparentfolder = 0 } ;
                Folder progs = new () { codefolder = 2, name = "ПО", codeparentfolder = 1 };
                Folder docs = new() { codefolder = 3, name = "Документы", codeparentfolder = 2};
                Folder scada = new() { codefolder = 4, name = "СКАДА", codeparentfolder = 2 };
                Folder books = new() { codefolder = 5, name = "Книги", codeparentfolder = 1 };

                Models.File f1 = new() { codefile = 1, namefile = "Scneider", About = "About1", codetypefolder = 1, codefolder = 2, Content = "" };
                Models.File f2 = new() { codefile = 2, namefile = "Важное", About = "About2", codetypefolder = 1, codefolder = 3, Content = "" };
                Models.File f3 = new() { codefile = 3, namefile = "Срочное", About = "About3", codetypefolder = 2, codefolder = 3, Content = "" };
                Models.File f4 = new() { codefile = 4, namefile = "Морковь", About = "About4", codetypefolder = 1, codefolder = 1, Content = "" };
                Models.File f5 = new() { codefile = 5, namefile = "Свекла", About = "About5", codetypefolder = 1, codefolder = 1, Content = "" };
                Models.File f6 = new() { codefile = 6, namefile = "Над пропастью во ржи", About = "About6", codetypefolder = 5, codefolder = 2, Content = "" };
                Models.File f7 = new() { codefile = 7, namefile = "Война и мир", About = "About7", codetypefolder = 2, codefolder = 5, Content = "" };
                Models.File f8 = new() { codefile = 8, namefile = "iFix", About = "About8", codetypefolder = 2, codefolder = 3, Content = "" };

                Extension e1 = new()
                {
                    codetypefolder = 1,
                    Type = "файл1",
                    icone = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAAXN" +
                    "SR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACJSURBVEhL7ZC9CYBADEbP2iEEp7JxAPdwBZex02EcQ/0iBCzC/ZlTkDx4RYrLC+cMwzB+QQ1bRR" +
                    "tYwSAbPJQdoRe6bIbS41x32MEg9NULlJakStEBRqMRT44yFF+htDRkdpTJiT+OMilxtSgTE1ePMr54sSgjxYtHmXv8tShD8Qn212QYxnc4dwKskJKEHrOFUQAAAABJRU5ErkJggg=="
                };
                Extension e2 = new()
                {
                    codetypefolder = 2,
                    Type = "файл2",
                    icone = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAAXN" +
                    "SR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACJSURBVEhL7ZC9CYBADEbP2iEEp7JxAPdwBZex02EcQ/0iBCzC/ZlTkDx4RYrLC+cMwzB+QQ1bRR" +
                    "tYwSAbPJQdoRe6bIbS41x32MEg9NULlJakStEBRqMRT44yFF+htDRkdpTJiT+OMilxtSgTE1ePMr54sSgjxYtHmXv8tShD8Qn212QYxnc4dwKskJKEHrOFUQAAAABJRU5ErkJggg=="
                };

                db.Папки.AddRange(root, progs, docs, scada, books);
                db.Файлы.AddRange(f1, f2, f3, f4, f5, f6, f7, f8);
                db.Расширения.AddRange(e1, e2);
                db.SaveChanges();
            }
        }
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        //--------------------------------------------------------------------------------------------------
        public ActionResult Folder()
        {
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
                      join Файл in db.Файлы on Папка.codefolder equals Файл.codefolder
                      //toDO//join Расширение in db.Расширения on Файл.КодТипаФайла equals Расширение.КодТипаФайла
                      select new
                      {
                          КодПапки = Папка.codefolder,
                          Название = Папка.name,
                          КодРодительскойПапки = Папка.codeparentfolder,
                          НазваниеФайла = Файл.namefile,
                          КодФайла = Файл.codefile
                      };

            foreach (var u in FFE)
            Console.WriteLine($"{u.КодПапки} - {u.Название} - {u.КодРодительскойПапки} - {u.НазваниеФайла} - {u.КодФайла}");
            return View();
        }

        public ActionResult FolderFileTree()//дерево
        {
            List<Объединенная> combinedTables = new List<Объединенная>();
            var connection = new Npgsql.NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            connection.Open();
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.codefolder, Папки.name, Папки.codeparentfolder, " +
                                                            "Файлы.namefile, Файлы.codefile, Расширения.icone " +
                                                            "FROM Папки LEFT JOIN Файлы ON Папки.codefolder = Файлы.codefolder " +
                                                                       "LEFT JOIN Расширения ON Файлы.codetypefolder = Расширения.codetypefolder " +
                                                            "ORDER BY Папки.name, Папки.codeFolder, Файлы.namefile, Файлы.codefile", connection);
                var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Folder table1 = new Folder
                            {
                                codefolder = reader.GetInt32(0),
                                name = reader.GetString(1),
                                codeparentfolder = reader.GetInt32(2),
                                // Set other properties
                            };
                //при добавлении папки которая не содержит файл
                Models.File table2 = new()
                            {
                                namefile = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                codefile = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
                                // Set other properties
                            };
                            Extension table3 = new()
                            {
                                icone = reader.IsDBNull(5) ? "" : reader.GetString(5),
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
            var command = new Npgsql.NpgsqlCommand("SELECT  Папки.CodeFolder, Папки.Название, Папки.КодРодительскойПапки, " +
                                                            "Файлы.Название, Файлы.КодФайла FROM Папки LEFT JOIN Файлы ON Папки.CodeFolder = Файлы.CodeFolder " +
                                                            "ORDER BY Папки.Название, Папки.CodeFolder, Файлы.Название, Файлы.КодФайла", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Folder table1 = new Folder
                {
                    codefolder = reader.GetInt32(0),
                    name = reader.GetString(1),
                    codeparentfolder = reader.GetInt32(2),
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

                Models.File table2 = new()
                {
                    namefile = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    codefile = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
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
            return View(await db.Файлы.Where(s => s.codefile == id).ToListAsync());
        }
        //--------------------------------------------------------------------------------------------------
        //public async Task<IActionResult> Icone(int id)
        //{
        //    return View(await db.Расширения.Where(s => s.КодТипаФайла == id).ToListAsync());
        //}
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Icone()//тестил иконку - потом удалить
        {
            return View(await db.Расширения.Where(s => s.codetypefolder == 1).ToListAsync());
        }

//--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Folder папка)
        {
            db.Папки.Add(папка);
            await db.SaveChangesAsync();
            return RedirectToAction("FolderFileTree");
        }

//--------------------------------------------------------------------------------------------------
                [HttpPost]
                public async Task<IActionResult> DeleteFolder(Folder папка)
                {
                    if (папка.codefolder != null)
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
                                                                             //если нет - то добавляет новую,
                                                                             //а может только не что не нуллабл требуются
        {
                db.Папки.Update(сущность.ПапкаИзОбъединенной);
                await db.SaveChangesAsync();
                return RedirectToAction("FolderFileTree");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFile(Объединенная сущность)  //из формы на странице Rename,
                                                                            //если есть все данные в форме то редактирует запись,
                                                                            //если нет - то добавляет новую
                                                                                                                                                 //а может только не что не нуллабл требуются
        {
            db.Файлы.Update(сущность.ФайлИзОбъединенной);
            await db.SaveChangesAsync();
            return RedirectToAction("FolderFileTree");
        }
        [HttpPost]
        public async Task<IActionResult> Rename(Объединенная сущность)
        {
            if (сущность.ПапкаИзОбъединенной != null)
                { сущность.ПапкаИзОбъединенной = await db.Папки.FirstOrDefaultAsync(p => p.codefolder == сущность.ПапкаИзОбъединенной.codefolder); }
            if (сущность.ФайлИзОбъединенной != null)
                { сущность.ФайлИзОбъединенной = await db.Файлы.FirstOrDefaultAsync(p => p.codefile == сущность.ФайлИзОбъединенной.codefile);}
            return View(сущность);
        }
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Rename(int? id)
        {
            if (id != null)
            {
                Объединенная сущность = new()
                { ПапкаИзОбъединенной = await db.Папки.FirstOrDefaultAsync(p => p.codefolder == id),
                  ФайлИзОбъединенной = await db.Файлы.FirstOrDefaultAsync(p => p.codefile == id) };
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
                public async Task<IActionResult> DeleteFile(Models.File файл)
                {
                    if (файл.codefile!= null)
                    {
                        db.Entry(файл).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    return NotFound();
                }

        //--------------------------------------------------------------------------------------------------
        //от исходного проекта по аннуитентным платежам
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