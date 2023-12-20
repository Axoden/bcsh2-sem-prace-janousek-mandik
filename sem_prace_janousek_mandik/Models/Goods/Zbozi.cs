using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi
	{
		[Browsable(false)]
		public int IdZbozi { get; set; }

		[Required]
		[RegularExpression("^.{3,}$", ErrorMessage = "Musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Název: ")]
		public string? Nazev { get; set; }

		[Required]
		[Range(0, 9999999)]
		[DisplayName("Jednotková cena [Kč]: ")]
		public float? JednotkovaCena { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		[DisplayName("Počet kusů na skladě: ")]
		public int PocetNaSklade { get; set; }

		[Required]
		[Range(0, Int64.MaxValue)]
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