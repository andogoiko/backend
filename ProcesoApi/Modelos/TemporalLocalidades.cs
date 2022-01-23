using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TemporalLocalidades
    {
        [Key, ForeignKey("Localidades")]
        public string Localidad { get; set; }
        public double? Temperatura { get; set; }
        public double? VelViento { get; set; }
        public double? Precipitaciones { get; set; }
        public double? Humedad { get; set; }

    }