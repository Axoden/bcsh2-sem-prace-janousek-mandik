using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Supplier
{
    public class Dodavatele
    {
        [Browsable(false)]
        public int IdDodavatele { get; set; }

        [Required]
		[RegularExpression("^.{3,}$", ErrorMessage = "Musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Název: ")]
		public string? Nazev { get; set; }

        [Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Jméno: ")]
		public string? Jmeno { get; set; }

        [Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
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
