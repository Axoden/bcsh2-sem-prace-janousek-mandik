using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi
	{
		[Browsable(false)]
		public int IdZbozi { get; set; }

		[Required]
		public string? Nazev { get; set; }

		[Required]
		public float? JednotkovaCena { get; set; }

		[Required]
		public int PocetNaSklade { get; set; }

		[Required]
		public int CarovyKod { get; set; }

        public string? Poznamka { get; set; }

		[Browsable(false)]
		public int IdDodavatele { get; set; }

		[Browsable(false)]
		public int IdUmisteni { get; set; }

		[Browsable(false)]
		public int IdKategorie { get; set; }
    }
}
