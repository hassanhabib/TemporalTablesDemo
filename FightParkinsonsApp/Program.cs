using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FightParkinsonsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var student = new Student
            {
                Id = 1,
                Name = "Michael J. Fox (New)"
            };

            var broker = new StorageBroker();

            IQueryable<Student> students = 
                broker.Students.TemporalAll();

            Console.Write(student);
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StorageBroker : DbContext
    {
        public StorageBroker() =>
            this.Database.Migrate();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FightParkinsonsDb;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .ToTable(
                    name: "Students",
                    studentsTable => studentsTable.IsTemporal(s =>
                    {
                        s.HasPeriodEnd("Start")
                    }));
        }

        public DbSet<Student> Students { get; set; }
    }
}