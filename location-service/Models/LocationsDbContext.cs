using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace location_service.Models;

public partial class LocationsDbContext : DbContext
{
    public LocationsDbContext()
    {
    }

    public LocationsDbContext(DbContextOptions<LocationsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("location_pkey");

            entity.ToTable("location", "location_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Latitude)
                .HasMaxLength(100)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(100)
                .HasColumnName("longitude");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
