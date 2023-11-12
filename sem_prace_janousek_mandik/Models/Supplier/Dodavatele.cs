using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Supplier
{
    public class Dodavatele
    {
        [Browsable(false)]
        public int IdDodavatele { get; set; }

        [Required]
		[DisplayName("Název: ")]
		public string? Nazev { get; set; }

        [Required]
		[DisplayName("Jméno: ")]
		public string? Jmeno { get; set; }

        [Required]
		[DisplayName("Příjmení: ")]
		public string? Prijmeni { get; set; }

        [Required]
        [Phone]
		[DisplayName("Telefon: ")]
		public string? Telefon { get; set; }

        [Required]
        [EmailAddress]
		[DisplayName("Email: ")]
		public string? Email { get; set; }

        [Browsable(false)]
        public int IdAdresy { get; set; }
    }
}
