using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("funkcije")]
    public class Funkcija
    {
        [Key]
        [Display(Name ="Šifra")]
        [Required(ErrorMessage = "{0} je obavezna")]
        [StringLength(10, ErrorMessage = " {0} mora biti duljine maksimalno {1} znakova")]
        public string Sifra { get; set; }

        [Display(Name = "Naziv")]
        [Required(ErrorMessage = " {0} je obavezno")]
        [StringLength(255, ErrorMessage = "{0} mora biti duljine maksimalno {1} znakova")]
        public string Naziv { get; set; }

        [Display(Name = "Aktivan")]
        [Required(ErrorMessage = " {0} je obavezna")]
        public bool Aktivnost { get; set; }
    }
}