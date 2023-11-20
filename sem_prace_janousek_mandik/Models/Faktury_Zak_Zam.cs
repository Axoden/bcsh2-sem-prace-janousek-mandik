using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models
{
	public class Faktury_Zak_Zam
	{
		[Browsable(false)]
		public int IdFaktury { get; set; }

		[Required]
		[DisplayName("Číslo faktury: ")]
		public int CisloFaktury { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Datum vystavení: ")]
		public DateOnly? DatumVystaveni { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Datum splatnosti: ")]
		public DateOnly? DatumSplatnosti { get; set; }

		[Required]
		[DisplayName("Částka za objednané zboží")]
		public float CastkaObjednavka { get; set; }

		[Required]
		[DisplayName("Částka za dopravu: ")]
		public float CastkaDoprava { get; set; }

		[Required]
		[DisplayName("DPH: ")]
		public float Dph { get; set; }

		[Required]
		[DisplayName("Zákazník: ")]
		public string? ZakaznikJmeno { get; set; }

		[Required]
		[DisplayName("Adresa zákazníka: ")]
		public string? AdresaZakaznika { get; set; }

		[Required]
		[DisplayName("Vytvořil zaměstnanec: ")]
		public string? ZamestnanecJmeno { get; set; }
	}
}