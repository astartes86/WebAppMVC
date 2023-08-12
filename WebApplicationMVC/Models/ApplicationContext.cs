using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Папка> Папки { get; set; } = null!;//создаем таблмчку для создания посгре базе

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Проводник3;Username=postgres;Password=Coraks_86_");
        }
    }
}
