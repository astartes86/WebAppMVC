using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebApplicationMVC.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Folder> Папки { get; set; } = null!;//создаем таблмчку для создания посгре базе
        public DbSet<File> Файлы { get; set; } = null!;
        public DbSet<Extension> Расширения { get; set; } = null!;

        //------------------------------------ вариант где задаем строку подключения при вызове
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)//так надо чтоб строка подключения заработала

        {
            Database.EnsureCreated();
        }
    }
        //------------------------------------вариант где здесь задем строку
        //public ApplicationContext()
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //optionsBuilder.LogTo(Console.WriteLine);
        //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Проводник6;Username=postgres;Password=Coraks_86_");
        //}
}


