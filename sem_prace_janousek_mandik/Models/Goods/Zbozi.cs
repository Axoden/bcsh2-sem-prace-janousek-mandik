using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi
	{
		[Browsable(false)]
		public int IdZbozi { get; set; }

		[Required]
		[DisplayName("Název: ")]
		public string? Nazev { get; set; }

		[Required]
		[Range(0, float.MaxValue)]
		[DisplayName("Jednotková cena: ")]
		public float? JednotkovaCena { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		[DisplayName("Počet na skladě: ")]
		public int PocetNaSklade { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		[DisplayName("Čárový kód: ")]
		public string? CarovyKod { get; set; }

		[DisplayName("Poznámka: ")]
		public string? Poznamka { get; set; }

		[Browsable(false)]
		public int IdDodavatele { get; set; }

		[Browsable(false)]
		public int IdUmisteni { get; set; }

		[Browsable(false)]
		public int IdKategorie { get; set; }

        [Browsable(false)]
        public int IdSouboru { get; set; }
    }
}