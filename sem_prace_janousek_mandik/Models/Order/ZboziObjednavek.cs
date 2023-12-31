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
		[Range(1, 9999)]
		public int Mnozstvi { get; set; }

		[Required]
		[Range(0, 9999999)]
		[DisplayName("Jednotková cena [Kč]: ")]
		public float JednotkovaCena { get; set; }

		[Browsable(false)]
		public int IdObjednavky { get; set; }

		[Browsable(false)]
		public int IdZbozi { get; set; }
    }
}
