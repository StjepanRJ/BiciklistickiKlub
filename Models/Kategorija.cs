using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("Kategorija")]
    public class Kategorija
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Naziv { get; set; }

        public virtual ICollection<Tema> Temas { get; set; }
    }
}