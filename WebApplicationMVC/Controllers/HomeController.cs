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
                Objects o1 = new() {  type = "Деталь", product = "СЕ.1235.01.00.001" } ;
                Objects o2 = new () {  type = "Материал по КД", product = "СЕ.1235.01.00.001" };
                Objects o3 = new() {  type = "Документ", product = "СЕ.1235.01.00.001" };

                Models.Attributes f1 = new() {  name = "Наименование", value = "Фланец"};
                Models.Attributes f2 = new() {  name = "Раздел спецификации", value = "Сборочные единицы" };
                Models.Attributes f3 = new() { name = "Марка материала", value = "Сталь 09Г2С" };

                db.Obj.AddRange(o1, o2, o3);
                db.Attr.AddRange(f1, f2, f3);
                db.SaveChanges();
            }
        }

        //--------------------------------------------------------------------------------------------------

        public ActionResult Tree()//дерево
        {
            List<United> combinedTables = new List<United>();
            var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            connection.Open();
            var command = new SqlCommand("SELECT  Obj.id, Obj.childId, Obj.product, " +
                                                            "Attr.name, Attr.objectId, Attr.value, Links.childId, Links.icone " +
                                                            "FROM Obj LEFT JOIN Attr ON Obj.id = Attr.id " +
                                                                       "LEFT JOIN Links ON Attr.parentId = Links.parentId " +
                                                            "ORDER BY Obj.childId, Obj.codeFolder, Attr.name, Attr.objectId", connection);
                var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Objects table1 = new Objects
                            {
                                id = reader.GetInt32(0),
                                type = reader.GetString(1),
                                product = reader.GetString(2),
                                // Set other properties
                            };
                //при добавлении папки которая не содержит файл
                Models.Attributes table2 = new()
                            {
                                name = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                objectId = reader.IsDBNull(4) ? -1 : reader.GetInt32(4),
                                value= reader.IsDBNull(5) ? "" : reader.GetString(5),
                                // Set other properties
                            };
                            Links table3 = new()
                            {
                                parentId = reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                                childId = reader.IsDBNull(7) ? -1 : reader.GetInt32(7),
                                // Set other properties
                            };
                combinedTables.Add(new United { ObjFromUnited = table1, AttrFromUnited = table2, LinksFromUnited = table3 });
                        }
            ListUnited model = new() { LUnited = combinedTables };
            
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
            return View(await db.Attr.Where(s => s.objectId == id).ToListAsync());
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
                { entity.AttrFromUnited = await db.Attr.FirstOrDefaultAsync(p => p.objectId == entity.AttrFromUnited.objectId);}
            return View(entity);
        }
        //--------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Rename(int? id)
        {
            if (id != null)
            {
                United ent = new()
                { ObjFromUnited = await db.Obj.FirstOrDefaultAsync(p => p.id == id),
                  AttrFromUnited = await db.Attr.FirstOrDefaultAsync(p => p.objectId == id) };
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
                    if (файл.objectId!= null)
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