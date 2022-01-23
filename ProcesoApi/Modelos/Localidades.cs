using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Localidades
    {
        [Key]
        public string Localidad { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [Required, ForeignKey("Provincias")]
        public string Provincia { get; set; }

        [Required, ForeignKey("ZonasRegion")]
        public string Zona { get; set; }

    }