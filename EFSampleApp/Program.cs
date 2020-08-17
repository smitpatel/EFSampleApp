using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFSampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var db = new MyContext())
            {
                // Recreate database
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Seed database
                var peanut = new Plant { Name = "Peanut" };
                var grass = new Plant { Name = "Grass" };
                var plankton = new Plant { Name = "Plankton" };

                var peanutFood = new Food { PlantId = peanut.Id };
                var grassFood = new Food { PlantId = grass.Id };
                var planktonFood = new Food { PlantId = plankton.Id };

                var rabbit = new Rabbit { IsFurry = true, Name = "Buttercup", Food = { grassFood } };
                var crow = new Crow { CanFly = true, FlyingRange = 50, Name = "Joey", Food = { peanutFood } };
                var sardine = new Fish { Name = "Grim", Length = 9, Food = { planktonFood } };

                var rabbitFood = new Food { AnimalId = rabbit.Id };
                var sardineFood = new Food { AnimalId = sardine.Id };

                var penguine = new Penguin { CanFly = false, Height = 35, Name = "Woody", Food = { sardineFood } };
                var tiger = new Tiger
                {
                    IsFurry = true,
                    Name = "Atticus",
                    Food = { rabbitFood },
                    Details = new TigerDetails
                    {
                        Color = "Yellow",
                        Region = "Africa",
                    }
                };

                db.AddRange(peanut, grass, plankton, rabbit, crow, sardine, penguine, tiger);

                db.SaveChanges();
            }

            using (var db = new MyContext())
            {
                // Run queries
                var allAnimals = db.Set<Animal>().ToList();

                var allBirds = db.Set<Bird>().ToList();

                var allAnimalData = db.Set<Animal>().Select(e => new { e.Name }).ToList();
                var allBirdData = db.Set<Bird>().Select(e => new { e.Name, e.CanFly }).ToList();

                var collectionOnAnimals = db.Set<Animal>().Select(e => new { e.Name, e.Food }).ToList();
            }

            Console.WriteLine("Program finished.");
        }
    }


    public class MyContext : DbContext
    {
        private static ILoggerFactory ContextLoggerFactory
            => LoggerFactory.Create(b =>
            {
                b
                .AddConsole()
                .AddFilter("Microsoft.EntityFrameworkCore.Command", LogLevel.Debug);
            });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Select 1 provider
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=_ModelApp;Trusted_Connection=True;Connect Timeout=5;ConnectRetryCount=0")
                //.UseSqlite("filename=_modelApp.db")
                //.UseInMemoryDatabase(databaseName: "_modelApp")
                //.UseCosmos("https://localhost:8081", @"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "_ModelApp")
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(ContextLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure model
            modelBuilder.Entity<Animal>();
            modelBuilder.Entity<Mammal>();
            modelBuilder.Entity<Rabbit>();
            modelBuilder.Entity<Tiger>().OwnsOne(e => e.Details);
            modelBuilder.Entity<Bird>();
            modelBuilder.Entity<Crow>();
            modelBuilder.Entity<Penguin>();
            modelBuilder.Entity<Fish>();
            modelBuilder.Entity<Plant>();
        }
    }

    [Table("Plants")]
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("FoodItems")]
    public class Food
    {
        public int Id { get; set; }
        public int? AnimalId { get; set; }
        public int? PlantId { get; set; }
        public ICollection<Animal> Animals { get; set; }
    }

    [Table("Animals")]
    public abstract class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Food> Food { get; set; } = new List<Food>();
    }

    [Table("Mammals")]
    public abstract class Mammal : Animal
    {
        public bool IsFurry { get; set; }
    }

    [Table("Birds")]
    public abstract class Bird : Animal
    {
        public bool CanFly { get; set; }
    }

    [Table("Tigers")]
    public class Tiger : Mammal
    {
        public TigerDetails Details { get; set; }
    }

    public class TigerDetails
    {
        public string Region { get; set; }
        public string Color { get; set; }
    }

    [Table("Rabbits")]
    public class Rabbit : Mammal
    {
    }

    [Table("Crows")]
    public class Crow : Bird
    {
        public double FlyingRange { get; set; }
    }

    [Table("Penguins")]
    public class Penguin : Bird
    {
        public int Height { get; set; }
    }

    [Table("Fishes")]
    public class Fish : Animal
    {
        public double Length { get; set; }
    }
}
