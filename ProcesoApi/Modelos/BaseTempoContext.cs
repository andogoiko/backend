using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BaseTempoContext : DbContext
    {

        public DbSet<ZonasRegion> ZonasRegion { get; set; }

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