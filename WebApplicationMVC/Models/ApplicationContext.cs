using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Папка> Папки { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Проводник;Username=postgres;Password=Coraks_86_");
        }
    }
}
