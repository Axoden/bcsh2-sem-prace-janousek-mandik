using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Payment
{
	public class PlatbyCustomerForm
	{
		[Browsable(false)]
		public int IdPlatby { get; set; }

		[Required]
		[DisplayName("Částka: ")]
		public float Castka { get; set; }

		[Required]
		[DisplayName("Typ platby: ")]
		public string TypPlatby { get; set; }

		[DisplayName("Variabilní symbol: ")]
		public string? VariabilniSymbol { get; set; }

		[Browsable(false)]
		public int IdFaktury { get; set; }
	}
}
