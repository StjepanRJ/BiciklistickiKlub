using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("lijecnicki")]
    public class Lijecnicki
    {
        [Key]
        [Display(Name = "ID liječnićkog")]
        public int Id { get; set; }

        [Column("korisnicko_ime")]
        [Display(Name = "Korisničko ime")]
        [Required]
        public string KorisnickoIme { get; set; }

        [Column("naziv_bolnice")]
        [Display(Name = "Naziv bolnice")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = " {0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string NazivBolnice { get; set; }

        [Column("naziv_mjesta")]
        [Display(Name = "Mjesto")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = " {0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Mjesto { get; set; }

        [Column("datum_lijecnickog")]
        [Display(Name = "Datum liječnićkog")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0} je obavezan")]
        [DataType(DataType.Date)]
        public DateTime DatumLijecnickog { get; set; }

        [Column("obavljen")]
        [Display(Name = "Obavljen")]
        public bool Obavljen { get; set; }
    }
}