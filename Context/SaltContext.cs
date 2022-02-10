using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using api.Models;

namespace api.Context
{
    public partial class SaltContext : DbContext
    {
        public SaltContext()
        {
        }

        public SaltContext(DbContextOptions<SaltContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Salt> Salts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-MEOM9R0\\SQLEXPRESS;Database=SharedData;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Salt>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__salt__497F6CB4AA8AA9B2");

                entity.ToTable("salt");

                entity.Property(e => e.Guid)
                    .ValueGeneratedNever()
                    .HasColumnName("guid");

                entity.Property(e => e.Salt1)
                    .HasMaxLength(36)
                    .HasColumnName("salt");

                entity.Property(e => e.UserGuid).HasColumnName("user_guid");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
