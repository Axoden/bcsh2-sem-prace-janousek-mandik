using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Employee
{
    public class Zamestnanci
    {
        [Browsable(false)]
        public int IdZamestnance { get; set; }

        [Required]
        public string? Jmeno { get; set; }

        [Required]
        public string? Prijmeni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly? DatumNarozeni { get; set; }

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

        [DataType(DataType.Password)]
        [Compare("Heslo", ErrorMessage = "Hesla se neshodují!")]
        public string? HesloZnova { get; set; }

        [Browsable(false)]
        public int IdAdresy { get; set; }

        [Browsable(false)]
        public int IdPozice { get; set; }
    }
}
