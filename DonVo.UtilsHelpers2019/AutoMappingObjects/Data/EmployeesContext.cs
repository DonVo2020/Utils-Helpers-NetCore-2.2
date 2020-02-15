using AutoMappingObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoMappingObjects.Data
{
    public class EmployeesContext : DbContext
    {
        public EmployeesContext()
        {

        }

        public EmployeesContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.configurationString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName)
                .IsRequired(true)
                .HasMaxLength(60);

                entity.Property(e => e.LastName)
                .IsRequired(true)
                .HasMaxLength(60);

                entity.Property(e => e.Salary)
                .IsRequired(true);

                entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsRequired(false);

            });
        }
    }
}
