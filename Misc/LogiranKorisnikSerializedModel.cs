using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Misc
{
    public class LogiranKorisnikSerializedModel
    {
        public string KorisnickoIme { get; set; }
        public string PrezimeIme { get; set; }
        public string Ovlast { get; set; }

        internal void CopyFromUser(LogiranKorisnik user)
        {
            this.KorisnickoIme = user.KorisnickoIme;
            this.PrezimeIme = user.PrezimeIme;
            this.Ovlast = user.Ovlast;
        }
    }
}