using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFSampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new MyContext())
            {
                // Recreate database
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Seed database


                db.SaveChanges();
            }

            using (var db = new MyContext())
            {
                // Run queries
                var query = db.Blogs.ToList();
            }

            Console.WriteLine("Program finished.");
        }
    }


    public class MyContext : DbContext
    {
        private static ILoggerFactory LoggerFactory => new LoggerFactory().AddConsole(LogLevel.Trace);

        // Declare DBSets
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Select 1 provider
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=_ModelApp;Trusted_Connection=True;Connect Timeout=5;ConnectRetryCount=0")
                //.UseSqlite("filename=_modelApp.db")
                //.UseInMemoryDatabase(databaseName: "_modelApp")
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(LoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure model
        }
    }

    public class Blog
    {
        public int Id { get; set; }
    }
}
