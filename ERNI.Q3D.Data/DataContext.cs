﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ERNI.Q3D.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrintJob>().Property(_ => _.Name).HasMaxLength(60);
            modelBuilder.Entity<User>().HasIndex(_ => _.Name).IsUnique();
        }

        public DbSet<PrintJob> PrintJobs { get; set; }

        public DbSet<User> Users { get; set; }
    }

    public class PrintJob : EntityBase
    {
        public PrintJobData Data { get; set; }

        public User Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public double FilamentLength { get; set; }

        public TimeSpan PrintTime { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string FileName { get; set; }

        public DateTime? PrintStartedAt { get; set; }

        public string SubjectLink { get; set; }

        public bool IsFinished { get; set; }

        public FilamentType FilamentType { get; set; }
    }

    public enum FilamentType
    {
        Unknown,
        PLA,
        ABS
    }

    public class PrintJobData : EntityBase
    {
        public byte[] Data { get; set; }
    }


    public class User : EntityBase
    {
        public string Name { get; set; }
    }

    public abstract class EntityBase
    {
        public long Id { get; set; }
    }

    public class DesignContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ERNI.3DPrintQueue;Trusted_Connection=True");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
