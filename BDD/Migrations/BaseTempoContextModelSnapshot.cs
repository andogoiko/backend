﻿// <auto-generated />
using System;
using BDD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BDD.Migrations
{
    [DbContext(typeof(BaseTempoContext))]
    partial class BaseTempoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BDD.Localidades", b =>
                {
                    b.Property<string>("Localidad")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Baliza")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Latitud")
                        .HasColumnType("float");

                    b.Property<double?>("Longitud")
                        .HasColumnType("float");

                    b.Property<string>("Provincia")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Localidad");

                    b.HasIndex("Provincia");

                    b.ToTable("Localidades");
                });

            modelBuilder.Entity("BDD.Provincias", b =>
                {
                    b.Property<string>("Provincia")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Provincia");

                    b.ToTable("Provincias");
                });

            modelBuilder.Entity("BDD.TemporalLocalidades", b =>
                {
                    b.Property<string>("Localidad")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double?>("Humedad")
                        .HasColumnType("float");

                    b.Property<double?>("Precipitaciones")
                        .HasColumnType("float");

                    b.Property<double?>("Temperatura")
                        .HasColumnType("float");

                    b.Property<double?>("VelViento")
                        .HasColumnType("float");

                    b.HasKey("Localidad");

                    b.ToTable("TemporalLocalidades");
                });

            modelBuilder.Entity("BDD.Localidades", b =>
                {
                    b.HasOne("BDD.Provincias", "Provincias")
                        .WithMany("Localidades")
                        .HasForeignKey("Provincia")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provincias");
                });

            modelBuilder.Entity("BDD.TemporalLocalidades", b =>
                {
                    b.HasOne("BDD.Localidades", "LocalidadFK")
                        .WithMany()
                        .HasForeignKey("Localidad")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LocalidadFK");
                });

            modelBuilder.Entity("BDD.Provincias", b =>
                {
                    b.Navigation("Localidades");
                });
#pragma warning restore 612, 618
        }
    }
}
