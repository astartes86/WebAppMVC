using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models;

//var builder = WebApplication.CreateBuilder(args);
var builder = WebApplication.CreateBuilder();
// получаем строку подключения из файла конфигурации
//string connection = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));//(options => options.UseNpgsql(connection));


builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
IConfiguration configuration = app.Configuration;

//app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"); // /{id?}");

app.Run();
