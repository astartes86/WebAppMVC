using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Папка> Папки { get; set; } = null!;//создаем таблмчку для создания посгре базе
        public DbSet<Файл> Файлы { get; set; } = null!;
        public DbSet<Расширение> Расширения { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Проводник5;Username=postgres;Password=Coraks_86_");
        }
    }
}
