using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Employee
{
    public class Zamestnanci
    {
		[Browsable(false)]
        public int IdZamestnance { get; set; }

        [Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Jméno: ")]
        public string? Jmeno { get; set; }

        [Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Příjmení: ")]
        public string? Prijmeni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Datum narození: ")]
		public DateOnly? DatumNarozeni { get; set; }

		[Required]
        [Phone]
        [DisplayName("Telefon: ")]
        public string? Telefon { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email: ")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("Heslo: ")]
        public string? Heslo { get; set; }

        [DataType(DataType.Password)]
        [Compare("Heslo", ErrorMessage = "Hesla se neshodují!")]
        [DisplayName("Heslo znovu: ")]
        public string? HesloZnova { get; set; }

        [Browsable(false)]
        public int IdAdresy { get; set; }

        [Browsable(false)]
        public int IdPozice { get; set; }
    }
}
