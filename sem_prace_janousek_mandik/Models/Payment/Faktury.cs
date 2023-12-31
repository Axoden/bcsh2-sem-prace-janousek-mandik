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
		[Range(90000, 99999)]
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
		[Range(0, 9999999)]
		[DisplayName("Celková částka za objednávku [Kč]: ")]
		public float CastkaObjednavka { get; set; }

		[Required]
		[Range(0, 999999)]
		[DisplayName("Částka za dopravu [Kč]: ")]
		public float CastkaDoprava { get; set; }

		[Required]
		[Range(0, 100)]
		[DisplayName("DPH [%]: ")]
		public float Dph { get; set; }
    }
}
