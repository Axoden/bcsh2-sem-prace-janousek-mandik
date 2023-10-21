using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Zakaznici
    {
        [Browsable(false)]
        public int IdZakaznika { get; set; }

        [Required]
        public string? Jmeno { get; set; }

        [Required]
        public string? Prijmeni { get; set; }

        [Required]
        [Phone]
        public string? Telefon { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Heslo { get; set; }

        /*[Compare("Password")]
        public string ConfirmPassword { get; set; } Když bude čas, tak můžeme přidat kontrolu hesla*/

        [Browsable(false)]
        public int IdAdresy { get; set; }
    }
}
