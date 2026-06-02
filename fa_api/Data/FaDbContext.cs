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

        public virtual DbSet<FaWrDroughtAlert> FaWrDroughtAlert { get; set; }
        public virtual DbSet<FaWrReservoirAlert> FaWrReservoirAlert { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FaWrDroughtAlert>(entity =>
            {
                entity.HasIndex(e => e.AreaName)
                    .HasName("UQ_FA_WR_DroughtAlert_AreaName")
                    .IsUnique();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.CreateUserNo).IsUnicode(false);

                entity.Property(e => e.Severity).IsUnicode(false);

                entity.Property(e => e.UpdateUserNo).IsUnicode(false);
            });

            modelBuilder.Entity<FaWrReservoirAlert>(entity =>
            {
                entity.HasIndex(e => e.ReservoirName)
                    .HasName("UQ_FA_WR_ReservoirAlert_ReservoirName")
                    .IsUnique();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.CreateUserNo).IsUnicode(false);

                entity.Property(e => e.UpdateUserNo).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
