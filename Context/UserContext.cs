using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using api.Models;

namespace api.Context
{
    public partial class UserContext : DbContext
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-MEOM9R0\\SQLEXPRESS;Database=SharedData;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__user__497F6CB4B82497FD");

                entity.ToTable("user");

                entity.Property(e => e.Guid)
                    .ValueGeneratedNever()
                    .HasColumnName("guid");

                entity.Property(e => e.Mail)
                    .HasMaxLength(500)
                    .HasColumnName("mail");

                entity.Property(e => e.Password)
                    .HasMaxLength(32)
                    .HasColumnName("password")
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .HasMaxLength(500)
                    .HasColumnName("user_name");

                entity.Property(e => e.Role)
                    .HasMaxLength(500)
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
