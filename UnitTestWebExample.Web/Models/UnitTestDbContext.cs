using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UnitTestWebExample.Web.Models;

public partial class UnitTestDbContext : DbContext
{
    public UnitTestDbContext()
    {
    }

    public UnitTestDbContext(DbContextOptions<UnitTestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
