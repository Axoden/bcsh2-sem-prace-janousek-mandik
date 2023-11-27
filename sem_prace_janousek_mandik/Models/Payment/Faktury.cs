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
		[Range(0, float.MaxValue)]
		[DisplayName("Celková částka za objednávku: ")]
		public float CastkaObjednavka { get; set; }

		[Required]
		[Range(0, float.MaxValue)]
		[DisplayName("Částka za dopravu: ")]
		public float CastkaDoprava { get; set; }

		[Required]
		[Range(0, float.MaxValue)]
		[DisplayName("DPH: ")]
		public float Dph { get; set; }
    }
}
