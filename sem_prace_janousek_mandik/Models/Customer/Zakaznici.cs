using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Customer
{
    public class Zakaznici
    {
        [Browsable(false)]
        public int IdZakaznika { get; set; }

        [Required]
        [DisplayName("Jméno:")]
        public string? Jmeno { get; set; }

        [Required]
		[DisplayName("Příjmení:")]
		public string? Prijmeni { get; set; }

        [Required]
        [Phone]
		[DisplayName("Telefon:")]
		public string? Telefon { get; set; }

        [Required]
        [EmailAddress]
		[DisplayName("Email:")]
		public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
		[DisplayName("Heslo:")]
		public string? Heslo { get; set; }

        [DataType(DataType.Password)]
        [Compare("Heslo", ErrorMessage = "Hesla se neshodují!")]
		[DisplayName("Heslo znovu:")]
		public string? HesloZnova { get; set; }

        [Browsable(false)]
        public int IdAdresy { get; set; }
    }
}
