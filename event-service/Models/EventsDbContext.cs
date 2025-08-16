using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace event_service.Models;

public partial class EventsDbContext : DbContext
{
    public EventsDbContext()
    {
    }

    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_pkey");

            entity.ToTable("event", "event_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Device)
                .HasMaxLength(100)
                .HasColumnName("device");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_type");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_pkey");

            entity.ToTable("type", "event_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
