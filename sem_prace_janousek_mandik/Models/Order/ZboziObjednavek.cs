using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class ZboziObjednavek
	{
		[Browsable(false)]
		public int IdZboziObjednavky { get; set; }

        [Required]
		[DisplayName("Množství: ")]
		[Range(1, 20)]
		public int Mnozstvi { get; set; }

		[Required]
		public float JednotkovaCena { get; set; }

		[Browsable(false)]
		public int IdObjednavky { get; set; }

		[Browsable(false)]
		public int IdZbozi { get; set; }
    }
}
