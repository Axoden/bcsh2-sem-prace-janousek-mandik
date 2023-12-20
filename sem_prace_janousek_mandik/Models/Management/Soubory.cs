using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Management
{
    public class Soubory
    {
        [Browsable(false)]
        public int IdSouboru { get; set; }

        [Required]
        [DisplayName("Název: ")]
        public string? Nazev { get; set; }

        [Required]
        [DisplayName("Typ souboru: ")]
        public string? TypSouboru { get; set; }

        [Required]
        [DisplayName("Přípona souboru: ")]
        public string? PriponaSouboru { get; set; }

        [DisplayName("Datum nahrání: ")]
        public DateTime DatumNahrani { get; set; }

        [DisplayName("Datum modifikace: ")]
        public DateTime DatumModifikace { get; set; }

		[Browsable(false)]
		public byte[]? Data { get; set; }

        [Browsable(false)]
        public int? IdZamestnance { get; set; }
    }
}
