using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiciklistickiKlub.Models
{
    public class PromjenaLozinke
    {
        [Display(Name = "Stara lozinka")]
        [Required(ErrorMessage = "Stara lozinka je potrebna")]
        [DataType(DataType.Password)]
        public string StaraLozinka { get; set; }

        [Display(Name = "Nova lozinka")]
        [Required(ErrorMessage = "Nova lozinka je potrebna")]
        [DataType(DataType.Password)]
        public string NovaLozinka { get; set; }

        [Display(Name = "Nova lozinka ponovljena")]
        [Required(ErrorMessage = "Nova lozinka je potrebna")]
        [Compare(otherProperty: "NovaLozinka", ErrorMessage = "Ne podudaraju se")]
        [DataType(DataType.Password)]
        public string StaraLozinkaPonovljena { get; set; }
    }
}