﻿using System;
using System.Text.Json.Serialization;
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

        public DbSet<Provincias> Provincias { get; set; }

        public DbSet<Localidades> Localidades { get; set; }

        public DbSet<TemporalLocalidades> TemporalLocalidades { get; set; }

        public string connString { get; private set; }

        public BaseTempoContext()
        {
            var database = "BDD06Andoitz";
            connString = $"Server=185.60.40.210\\SQLEXPRESS,58015;Database={database};User Id=sa;Password=Pa88word;MultipleActiveResultSets=true";
            //connString = $"Server=(localdb)\\mssqllocaldb;Database=PluebasDeKaka;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(connString);

    }

    public class Provincias
    {
        [Key]
        public string Provincia { get; set; }

        [JsonIgnore]
        public List<Localidades> Localidades { get; set; }
    }

    public class Localidades
    {
        [Key]
        public string Localidad { get; set; }

        public string Baliza { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [Required, ForeignKey("Provincias")]
        public string Provincia { get; set; }

        [JsonIgnore]
        public Provincias Provincias { get; set; }

    }

    public class TemporalLocalidades
    {
        [Key, ForeignKey("LocalidadFK")]
        public string Localidad { get; set; }
        public double? Temperatura { get; set; }
        public double? VelViento { get; set; }
        public double? Precipitaciones { get; set; }
        public double? Humedad { get; set; }

        [JsonIgnore]
        public virtual Localidades LocalidadFK { get; set; }
        //esta configuración para crear una relación 0:1 -> https://stackoverflow.com/questions/49305921/entity-framework-1-to-0-or-1-relationship-configuration
    }


}


