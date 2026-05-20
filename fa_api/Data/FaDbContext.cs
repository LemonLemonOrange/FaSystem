using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using fa_api.Models;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace fa_api.Data
{
    public partial class FaDbContext : DbContext
    {
        public FaDbContext()
        {
        }

        public FaDbContext(DbContextOptions<FaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FaWrSignalLevel> FaWrSignalLevel { get; set; }
        public virtual DbSet<FaWrSignalSnapshot> FaWrSignalSnapshot { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FaWrSignalLevel>(entity =>
            {
                entity.HasIndex(e => e.AlertIdentifier);

                entity.HasIndex(e => e.County);

                entity.HasIndex(e => e.EffectiveTime);

                entity.HasIndex(e => e.RecordTime);

                entity.HasIndex(e => e.SignalLevel);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RecordTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FaWrSignalSnapshot>(entity =>
            {
                entity.HasIndex(e => e.AreaName);

                entity.HasIndex(e => e.BatchId);

                entity.HasIndex(e => e.RecordTime);

                entity.Property(e => e.RecordTime).HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
