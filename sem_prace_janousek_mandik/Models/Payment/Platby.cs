using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Payment
{
	public class Platby
	{
        [Browsable(false)]
        public int IdPlatby { get; set; }

		[Required]
		[DisplayName("Datum platby: ")]
		public DateTime DatumPlatby { get; set; }

        [Required]
		[DisplayName("Částka: ")]
		[Range(0, float.MaxValue)]
		public float Castka { get; set; }

        [Required]
		[DisplayName("Typ platby: ")]
		public char? TypPlatby { get; set; }

		[DisplayName("Variabilní symbol: ")]
		[Range(0, 99999999999999)]
		public string? VariabilniSymbol { get; set; }

        [Browsable(false)]
        public int IdFaktury { get; set; }
    }
}
