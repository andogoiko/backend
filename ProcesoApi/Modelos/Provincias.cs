using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Provincias
    {
        [Key]
        public string Provincia { get; set; }

        public List<Localidades> Localidades { get; set; }
    }