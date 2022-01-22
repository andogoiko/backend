using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TemporalLocalidades
    {
        [Key, ForeignKey("LocalidadFK")]
        public string Localidad { get; set; }
        public double? Temperatura { get; set; }
        public double? VelViento { get; set; }
        public double? Precipitaciones { get; set; }
        public double? Humedad { get; set; }

        public virtual Localidades LocalidadFK { get; set; }
        //esta configuración para crear una relación 0:1 -> https://stackoverflow.com/questions/49305921/entity-framework-1-to-0-or-1-relationship-configuration
    }