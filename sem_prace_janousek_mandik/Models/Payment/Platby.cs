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
		public float Castka { get; set; }

        [Required]
		[DisplayName("Typ platby: ")]
		public char TypPlatby { get; set; }

		[DisplayName("Variabilní symbol: ")]
		public string? VariabilniSymbol { get; set; }

        [Browsable(false)]
        public int IdFaktury { get; set; }
    }
}
