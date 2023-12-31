using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Payment
{
	public class PlatbyForm
	{
		[Browsable(false)]
		public int IdPlatby { get; set; }

        [Required]
        [DisplayName("Datum platby: ")]
        public DateTime DatumPlatby { get; set; }

        [Required]
		[Range(0, 9999999)]
		[DisplayName("Částka: ")]
		public float Castka { get; set; }

		[Required]
		[DisplayName("Typ platby: ")]
		public string? TypPlatby { get; set; }

		[DisplayName("Variabilní symbol: ")]
		[Range(0, 9999999999999)]
		public string? VariabilniSymbol { get; set; }

		[Browsable(false)]
		public int IdFaktury { get; set; }
	}
}
