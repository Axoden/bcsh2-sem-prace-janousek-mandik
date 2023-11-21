using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Soubory
    {
        [Browsable(false)]
        public int IdSouboru { get; set; }

        [Required]
        public string? Nazev { get; set; }

		[Required]
		public string? TypSouboru { get; set; }

		[Required]
		public string? PriponaSouboru { get; set; }

		public DateTime DatumNahrani { get; set; }

		public DateTime DatumModifikace { get; set; }

		[Required]
		public byte[]? Data { get; set; }

		[Browsable(false)]
		public int? idZamestnance { get; set; }
	}
}
