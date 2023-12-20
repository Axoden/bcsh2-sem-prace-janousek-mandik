using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky
	{
        [Browsable(false)]
        public int IdObjednavky { get; set; }

        [Required]
		[DisplayName("Číslo objednávky: ")]
		public int CisloObjednavky { get; set; }

		[Required(ErrorMessage = "Datum přijetí je povinné")]
		[DisplayName("Datum přijetí: ")]
		public DateTime DatumPrijeti { get; set; }

		[DisplayName("Poznámka: ")]
		public string? Poznamka { get; set; }

		[Browsable(false)]
		public bool Uzavrena { get; set; }

		[Browsable(false)]
		public int IdFaktury { get; set; }

		[Browsable(false)]
		public int IdZakaznika { get; set; }

		[Browsable(false)]
		public int IdZamestnance { get; set; }
    }
}