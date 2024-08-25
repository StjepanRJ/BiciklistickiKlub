using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("Tema")]
    public class Tema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Naslov { get; set; }

        [ForeignKey("Kategorija")]
        public int KategorijaId { get; set; }

        public virtual Kategorija Kategorija { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}