using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SimuladoConcursos.Models;

namespace SimuladoConcursos.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "simulado.db");
            optionsBuilder.UseSqlite($"Data Source={databasePath};");
        }

        public static void InitializeDatabase()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>(entity =>
            {
                entity.OwnsMany(q => q.Opcoes, o =>
                {
                    o.WithOwner().HasForeignKey("QuestionId");
                    o.Property<int>("Id");
                    o.HasKey("Id");
                });

                entity.Property(q => q.RespostaCorreta)
                      .HasConversion(
                          v => v.ToUpper(),
                          v => v);
            });
        }
    }
}