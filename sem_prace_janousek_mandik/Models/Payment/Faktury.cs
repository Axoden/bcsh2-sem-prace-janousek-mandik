using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Payment
{
	public class Faktury
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
		[DisplayName("Celková částka za objednávku: ")]
		public float CastkaObjednavka { get; set; }

		[Required]
		[DisplayName("Částka za dopravu: ")]
		public float CastkaDoprava { get; set; }

		[Required]
		[DisplayName("DPH: ")]
		public float Dph { get; set; }
    }
}
