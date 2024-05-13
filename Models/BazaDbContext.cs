using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BazaDbContext : DbContext
    {
        public DbSet<Clan> PopisClanova { get; set; }
        public DbSet<Funkcija> PopisFunkcija { get; set; }
        public DbSet<Korisnik> PopisKorisnika { get; set; }
        public DbSet<Ovlast> PopisOvlasti { get; set; }
    }
}