using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Supplier
{
    public class Dodavatele
    {
        [Browsable(false)]
        public int IdDodavatele { get; set; }

        [Required]
        public string? Nazev { get; set; }

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

        [Browsable(false)]
        public int IdAdresy { get; set; }
    }
}
