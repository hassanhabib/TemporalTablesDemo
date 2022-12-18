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
            student = broker.Update(student).Entity;
            broker.SaveChanges();

            // modify record again
            student.Name = "M.J. Fox";
            broker.Update(student);
            broker.SaveChanges();

            // retrieve historical data
            IQueryable<Student> students =
                broker.Students.TemporalAll();

            // get the student id to show history for specific record
            int studentId = student.Id;

            // retrieve historical data with the ValidFrom / ValidTo
            IQueryable<StudentHistory> history = students.Where(student => student.Id == studentId)
                .Select(student => new StudentHistory
                {
                    Id = student.Id,
                    Name = student.Name,
                    PeriodStart = EF.Property<DateTime>(student, "PeriodStart"),
                    PeriodEnd = EF.Property<DateTime>(student, "PeriodEnd"),
                });

            history.ToList().ForEach(student =>
            {
                Console.WriteLine($"Id: {student.Id}, Name {student.Name}, ValidFrom: {student.PeriodStart}, ValidTo: {student.PeriodEnd}");
            });

            Console.WriteLine("Don't forget to donate to Michael J. Fox Foundation!");
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StudentHistory : Student
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
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