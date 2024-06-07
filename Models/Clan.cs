using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("clanovi")]
    public class Clan
    {
        [Key]
        [Display(Name = "ID ćlana")]
        public int Id { get; set; }


        [Display(Name = "Ime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = " {0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Ime { get; set; }

        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = " {0} mora biti duljine minimalno {2} a maksimalno {1} znakova")]
        public string Prezime { get; set; }
        public string PrezimeIme
        {
            get
            {
                return Prezime + " " + Ime;
            }
        }

        [Display(Name = "Spol")]
        public string Spol { get; set; }

        [Display(Name = "OIB")]
        [Required(ErrorMessage = "{0} je obavezno")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} mora biti duljine {1} znakova")]
        public string Oib { get; set; }

        [Column("datum_rodjenja")]
        [Display(Name = "Datum rođenja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0} je obavezan")]
        [DataType(DataType.Date)]
        public DateTime DatumRodjenja { get; set; }

        [Column("kategorija_clana")]
        [Display(Name = "Kategorija Člana")]
        public KategorijaClana KategorijaClana { get; set; }

        [Column("kategorija_clanstva")]
        [Display(Name = "Kategorija članstva")]
        public KategorijaClanstva KategorijaClanstva { get; set; }

        [Column("redovan_clan")]
        [Display(Name = "Redovan član")]
        public bool RedovanClan { get; set; }

        [Display(Name = "Funkcija")]
        [Column("funkcija_sifra")]
        [ForeignKey("UpisanaFunkcija")]
        public string SifraFunkcije { get; set; }

        public virtual Funkcija UpisanaFunkcija { get; set; }
    }
}