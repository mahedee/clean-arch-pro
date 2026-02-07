using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EduTrack.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API Configurations
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configure value objects with conversions
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasConversion(
                        v => v.Value,
                        v => FullName.Create(v));
                
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasConversion(
                        v => v.Value,
                        v => Email.Create(v));
                
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasConversion(
                        v => v != null ? v.Value : null,
                        v => v != null ? PhoneNumber.Create(v) : null);
                
                entity.Property(e => e.CurrentGPA)
                    .HasPrecision(3, 2)
                    .HasConversion(
                        v => v != null ? v.Value : (decimal?)null,
                        v => v.HasValue ? GPA.Create(v.Value) : null);
                
                // Configure Address as owned entity
                entity.OwnsOne(s => s.Address, addressBuilder =>
                {
                    addressBuilder.Property(a => a.Street).HasMaxLength(100);
                    addressBuilder.Property(a => a.Street2).HasMaxLength(100);
                    addressBuilder.Property(a => a.City).HasMaxLength(50);
                    addressBuilder.Property(a => a.State).HasMaxLength(2);
                    addressBuilder.Property(a => a.ZipCode).HasMaxLength(10);
                    addressBuilder.Property(a => a.Country).HasMaxLength(50);
                });
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Semester).HasMaxLength(50);
                entity.Property(e => e.Level).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
            });
        }
    }
}
