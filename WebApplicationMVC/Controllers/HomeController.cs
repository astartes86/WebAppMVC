using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMVC.Models;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

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
            if (!db.Obj.Any())
            {
                Objects o1 = new() { type = "Деталь", product = "СЕ.1235.01.00.001" } ;
                Objects o2 = new () { type = "Материал по КД", product = "Сталь 09Г2С" };
                Objects o3 = new() { type = "Документ1", product = "СЕ.1235.01.01.000 ВП" };
                Objects o4 = new() { type = "Документ2", product = "СЕ.1235.01.01.000 ВВ" };
                Objects o5 = new() { type = "Документ3", product = "СЕ.1235.01.01.000 ВР" };
                Objects o6 = new() { type = "Документ4", product = "СЕ.1235.01.01.000 ВЖ" };
                Objects o7 = new() { type = "Документ5", product = "СЕ.1235.01.01.000 ВК" };

                Models.Attributes f1 = new() {  name = "Наименование", value = "Фланец"};
                Models.Attributes f2 = new() {  name = "Раздел спецификации", value = "Сборочные единицы" };
                Models.Attributes f3 = new() { name = "Марка материала", value = "Сталь 09Г2С" };

                Models.Links l1 = new() { parentId = 3, childId = 4 };
                Models.Links l2 = new() { parentId = 4, childId = 5 };
                Models.Links l3 = new() { parentId = 4, childId = 6 };


                db.Obj.AddRange(o1, o2, o3, o4, o5, o6, o7);
                db.Attr.AddRange(f1, f2, f3);
                db.Links.AddRange(l1, l2, l3);
                db.SaveChanges();
            }
        }

        //--------------------------------------------------------------------------------------------------

        public ActionResult Tree()//дерево
        {
            List<United> combinedTables = new List<United>();
            var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            connection.Open();
            //var command = new SqlCommand("SELECT  Obj.id, Obj.type, Obj.product, " +
            //                                                " Links.parentId, Links.childId  " +
            //                                                "FROM Obj LEFT JOIN Links ON Obj.id = Links.parentId " +
            //
            //                                                "ORDER BY Links.parentId desc", connection);
            var command = new SqlCommand("SELECT  Links.parentId, Links.childId, Obj.id, Obj.type, Obj.product " +
                                            "FROM Links LEFT JOIN Obj ON Obj.id = Links.parentId ", connection);


            var reader = command.ExecuteReader();
            bool b=false;
            int root=0;
            while (reader.Read())
                        {
                            Objects table1 = new Objects
                            {
                                id = reader.IsDBNull(2) ? -1 : reader.GetInt32(2),
                                type = reader.IsDBNull(4) ? "" : reader.GetString(3),
                                product = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                // Set other properties
                            };
                            
                            Links table3 = new()
                            {
                                //id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                                parentId = reader.IsDBNull(0) ? -1 : reader.GetInt32(0),
                                childId = reader.IsDBNull(1) ? -1 : reader.GetInt32(1),
                                
                                // Set other properties
                            };

                            if (b==false) { root = (int)table3.parentId; b = true; }

                combinedTables.Add(new United { ObjFromUnited = table1, /*AttrFromUnited = table2,*/ LinksFromUnited = table3 });
            }
            ListUnited model = new() { LUnited = combinedTables, Seed = root };
            
            return View(model);//для вывода в виде дерева
        }

        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            return View(await db.Obj.ToListAsync());
        }
        //--------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> AttrContent(int id)
        {
            return View(await db.Attr.Where(s => s.id == id).ToListAsync());
        }
        //--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Objects папка)
        {
            db.Obj.Add(папка);
            await db.SaveChangesAsync();
            return RedirectToAction("Tree");
        }

//--------------------------------------------------------------------------------------------------
                [HttpPost]
                public async Task<IActionResult> ObjDelete(Objects ob)
                {
                    if (ob.id != null)
                    {
                        db.Entry(ob).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Tree");
                    }
                    return NotFound();
                    //return $"{папка.КодПапки}";
        }
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateFolder(United сущность) //из формы на странице Rename,
                                                                             //если есть все данные в форме то редактирует запись,
                                                                             //если нет - то добавляет новую,
                                                                             //а может только не что не нуллабл требуются
        {
                db.Obj.Update(сущность.ObjFromUnited);
                await db.SaveChangesAsync();
                return RedirectToAction("FolderFileTree");
        }
        //--------------------------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> UpdateFile(United entity)  //из формы на странице Rename,
                                                                            //если есть все данные в форме то редактирует запись,
                                                                            //если нет - то добавляет новую
                                                                            //а может только не что не нуллабл требуются
        {
            db.Attr.Update(entity.AttrFromUnited);
            await db.SaveChangesAsync();
            return RedirectToAction("FolderFileTree");
        }
        //--------------------------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Rename(United entity)
        {
            if (entity.ObjFromUnited != null)
                { entity.ObjFromUnited = await db.Obj.FirstOrDefaultAsync(p => p.id == entity.ObjFromUnited.id); }
            if (entity.AttrFromUnited != null)
                { entity.AttrFromUnited = await db.Attr.FirstOrDefaultAsync(p => p.id == entity.AttrFromUnited.id);}
            return View(entity);
        }
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Rename(int? id)
        {
            if (id != null)
            {
                United ent = new()
                { ObjFromUnited = await db.Obj.FirstOrDefaultAsync(p => p.id == id),
                  AttrFromUnited = await db.Attr.FirstOrDefaultAsync(p => p.id == id) };
                if (ent.ObjFromUnited != null) return View(ent);
                if (ent.AttrFromUnited != null) return View(ent);
            }
            return NotFound();
        }

        //--------------------------------------------------------------------------------------------------
        public IActionResult AttrDelete()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------
        public IActionResult ObjDelete()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        //--------------------------------------------------------------------------------------------------


        [HttpPost]
                public async Task<IActionResult> DeleteFile(Models.Attributes файл)
                {
                    if (файл.id!= null)
                    {
                        db.Entry(файл).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    return NotFound();
                }

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
   
}