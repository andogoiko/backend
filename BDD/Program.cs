using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static System.Console;
Console.Write("esta es la BDD");

namespace BDD
{

    public class BaseTempoContext : DbContext
    {

        public DbSet<ZonasRegion> ZonaRegion { get; set; }

        public DbSet<Provincias> Provincia { get; set; }

        public DbSet<Localidades> Localidad { get; set; }

        public DbSet<TemporalLocalidades> TemporalLocalidad { get; set; }

        public string connString { get; private set; }

        public BaseTempoContext()
        {
            var database = "BDD06Andoitz"; // "EF{XX}Nombre" => EF00Santi
            connString = $"Server=185.60.40.210\\SQLEXPRESS,58015;Database={database};User Id=sa;Password=Pa88word;MultipleActiveResultSets=true";
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Localidades>().HasIndex(m => new
        //     {

        //     }).IsUnique();
        // }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(connString);

    }
    public class ZonasRegion
    {
        [Key]
        public string Zona { get; set; }

        public List<Localidades> LocalidadRegi { get; set; }
    }

    public class Provincias
    {
        [Key]
        public string Provincia { get; set; }

        public List<Localidades> LocalidadProv { get; set; }
    }

    public class Localidades
    {
        [Key]
        public string Localidad { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Provincia { get; set; }
        public string Zona { get; set; }
        public ZonasRegion ZonaRegionFK { get; set; }

        public Provincias ProvinciaFK { get; set; }

    }

    public class TemporalLocalidades
    {
        [Key, ForeignKey("Localidades")]
        public string Localidad { get; set; }
        public double Temperatura { get; set; }
        public double VelViento { get; set; }
        public double Precipitaciones { get; set; }
        public double Humedad { get; set; }

        public virtual Localidades LocalidadFK { get; set; }
        //esta configuración para crear una relación 0:1 -> https://stackoverflow.com/questions/49305921/entity-framework-1-to-0-or-1-relationship-configuration
    }


}


