using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    [Table("Post")]
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Sadrzaj { get; set; }

        public string Pitanje { get; set; }

        public string KorisnickoImePitanje { get; set; }

        [Required]
        public DateTime DatumKreiranja { get; set; }

        [ForeignKey("Tema")]
        public int TemaId { get; set; }

        [Required]
        [StringLength(255)]
        public string KorisnickoIme { get; set; }

        public virtual Tema Tema { get; set; }
    }
}