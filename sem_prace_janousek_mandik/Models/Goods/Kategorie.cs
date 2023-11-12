using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Kategorie
	{
		[Browsable(false)]
		public int IdKategorie { get; set; }

		[Required]
		public string? Nazev { get; set; }

		[Required]
		[StringLength(4, MinimumLength = 3, ErrorMessage = "Délka názvu zkratky musí být mezi {2} a {1} znaky")]
		public string? Zkratka { get; set; }

        public string? Popis { get; set; }

		[Browsable(false)]
		public int IdNadrazeneKategorie { get; set; }
    }
}
