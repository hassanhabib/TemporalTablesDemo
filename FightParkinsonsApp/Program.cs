using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FightParkinsonsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // new record
            var student = new Student
            {
                Name = "Michael J. Fox (New)"
            };

            // add record
            var broker = new StorageBroker();
            broker.Add(student);
            broker.SaveChanges();

            // modify record
            student.Name = "Michael Fox";
            broker.Update(student);
            broker.SaveChanges();

            // retrieve historical data
            IQueryable<Student> students = 
                broker.Students.TemporalAll();


            Console.Write("Don't forget to donate to Michael J. Fox Foundation!");
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
                .ToTable(name: "Students", studentsTable => studentsTable.IsTemporal());
        }

        public DbSet<Student> Students { get; set; }
    }
}