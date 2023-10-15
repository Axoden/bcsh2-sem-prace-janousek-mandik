using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Zakaznici
    {
        public int IdZakaznika { get; set; }

        public string? Jmeno { get; set; }

        public string? Prijmeni { get; set; }

        public string? Telefon { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [Range(6, 999)]
        public string? Heslo { get; set; }

        public int IdAdresy { get; set; }
    }
}
