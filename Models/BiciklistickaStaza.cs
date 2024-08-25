using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("BiciklistickaStaza")]
    public class BiciklistickaStaza
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Ime { get; set; }

        [Required]
        public string GeoJson { get; set; } // Ovdje ćemo čuvati GeoJSON podatke

        [Required]
        public string UserId { get; set; }

        public DateTime Datum { get; set; }
    }
}