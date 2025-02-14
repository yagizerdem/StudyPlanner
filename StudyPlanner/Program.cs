using DataContext;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Service;
using System.Runtime.CompilerServices;

namespace StudyPlanner
{
    internal static class Program
    {

        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Initilize();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        
         
            

        }

        

        static void Initilize()
        {
            string docuementsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string dbPath = Path.Join(docuementsPath, "appService.db");

            Environment.SetEnvironmentVariable("APP_DB_PATH", dbPath, EnvironmentVariableTarget.Process);

            if (!File.Exists(dbPath))
            {
                File.Create(dbPath);
            }

            // register services

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

        }
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<AppDbContext>();
            services.AddSingleton<NotesController>();
            services.AddSingleton<StudyResourcesController>();
            services.AddSingleton<TimableEventController>();
        }

    }
}