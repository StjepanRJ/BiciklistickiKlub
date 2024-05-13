using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("korisnici")]
    public class Korisnik
    {
        public string KorisničkoIme { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string Prezime { get; set; }
        public string Ime { get; set; }

        public string PrezimeIme
        {
            get
            {
                return Prezime + " " + Ime;
            }
        }

        public string SifraOvlasti { get; set; }
        public virtual Ovlast Ovlas { get; set; }
        public string LozinkaUnos { get; set; }
        public string LozinkaUnos2 { get; set; }
    }
}