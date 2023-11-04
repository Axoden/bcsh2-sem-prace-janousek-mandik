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
		public string? Zkratka { get; set; }

        public string? Popis { get; set; }
    }
}
