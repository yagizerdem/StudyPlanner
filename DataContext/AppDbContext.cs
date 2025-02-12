using Microsoft.EntityFrameworkCore;
using Models;

namespace DataContext
{
    public class AppDbContext : DbContext
    {
       
        public DbSet<NotesModel> Notes { get; set; }
        public DbSet<UnitModel> Units { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
        public DbSet<ResourcesModel> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Retrieve the database path from an environment variable
            string dbPath = Environment.GetEnvironmentVariable("APP_DB_PATH", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(dbPath))
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dbPath = Path.Combine(documentsPath, "appService.db");
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

    }
}
