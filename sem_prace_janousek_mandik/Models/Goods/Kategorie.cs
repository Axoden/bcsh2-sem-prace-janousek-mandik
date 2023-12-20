using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Kategorie
	{
		[Browsable(false)]
		public int IdKategorie { get; set; }

		[Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Název: ")]
		public string? Nazev { get; set; }

		[Required]
		[StringLength(4, MinimumLength = 3, ErrorMessage = "Délka názvu zkratky musí být mezi {2} a {1} znaky")]
		[DisplayName("Zkratka: ")]
		public string? Zkratka { get; set; }

		[DisplayName("Popis: ")]
		public string? Popis { get; set; }

		[Browsable(false)]
		public int IdNadrazeneKategorie { get; set; }
    }
}
