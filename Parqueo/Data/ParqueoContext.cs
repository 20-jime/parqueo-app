using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Parqueo.Models;

namespace Parqueo.Data;

public partial class ParqueoContext : DbContext
{
    public ParqueoContext(DbContextOptions<ParqueoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PrqAutomovile> PrqAutomoviles { get; set; }

    public virtual DbSet<PrqIngresoAutomovile> PrqIngresoAutomoviles { get; set; }

    public virtual DbSet<PrqParqueo> PrqParqueos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("user_08", "PRQTipoAutomovil", new[] { "sedan", "4x4", "moto" })
            .HasPostgresEnum("user_08", "match_kind", new[] { "competition", "friendly" })
            .HasPostgresEnum("user_08", "match_status", new[] { "scheduled", "live", "finished" })
            .HasPostgresEnum("user_08", "sport_type", new[] { "soccer", "basketball", "baseball", "volleyball", "rugby", "hockey" })
            .HasPostgresEnum("user_25", "match_kind", new[] { "friendly", "official" })
            .HasPostgresEnum("user_25", "match_status", new[] { "scheduled", "live", "finished" })
            .HasPostgresEnum("user_25", "sport", new[] { "soccer", "basketball", "baseball", "volleyball", "rugby", "hockey" });

        modelBuilder.Entity<PrqAutomovile>(entity =>
        {
            entity.HasKey(e => e.IdAutomovil).HasName("prq_automoviles_pkey");

            entity.ToTable("prq_automoviles", "user_14");

            entity.Property(e => e.IdAutomovil).HasColumnName("id_automovil");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.Fabricante)
                .HasMaxLength(50)
                .HasColumnName("fabricante");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<PrqIngresoAutomovile>(entity =>
        {
            entity.HasKey(e => e.Consecutivo).HasName("prq_ingreso_automoviles_pkey");

            entity.ToTable("prq_ingreso_automoviles", "user_14");

            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.FechaHoraEntrada)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora_entrada");
            entity.Property(e => e.FechaHoraSalida)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora_salida");
            entity.Property(e => e.IdAutomovil).HasColumnName("id_automovil");
            entity.Property(e => e.IdParqueo).HasColumnName("id_parqueo");

            entity.HasOne(d => d.IdAutomovilNavigation).WithMany(p => p.PrqIngresoAutomoviles)
                .HasForeignKey(d => d.IdAutomovil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prq_ingreso_automoviles_id_automovil_fkey");

            entity.HasOne(d => d.IdParqueoNavigation).WithMany(p => p.PrqIngresoAutomoviles)
                .HasForeignKey(d => d.IdParqueo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prq_ingreso_automoviles_id_parqueo_fkey");
        });

        modelBuilder.Entity<PrqParqueo>(entity =>
        {
            entity.HasKey(e => e.IdParqueo).HasName("prq_parqueo_pkey");

            entity.ToTable("prq_parqueo", "user_14");

            entity.Property(e => e.IdParqueo).HasColumnName("id_parqueo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreProvincia)
                .HasMaxLength(50)
                .HasColumnName("nombre_provincia");
            entity.Property(e => e.PrecioPorHora)
                .HasPrecision(10, 2)
                .HasColumnName("precio_por_hora");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
