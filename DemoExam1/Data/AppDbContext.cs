using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoExam1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoExam1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RepairDb;Username=postgres;Password=qwerty123");
            }
        }
        public DbSet<RepairTask> Tasks => Set<RepairTask>();
        public DbSet<Client> Client => Set<Client>();
        public DbSet<Equipment> Equipment => Set<Equipment>();
        public DbSet<OrderSpare> OrderSpare => Set<OrderSpare>();
        public DbSet<User> User => Set<User>();
        public DbSet<TaskEquipment> TaskEquipment => Set<TaskEquipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEquipment>()
                .HasOne(x => x.Equipment)
                .WithMany()
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
