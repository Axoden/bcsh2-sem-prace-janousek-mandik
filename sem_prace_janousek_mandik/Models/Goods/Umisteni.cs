using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Umisteni
	{
        [Browsable(false)]
        public int IdUmisteni { get; set; }

        [Required]
		[DisplayName("Místnost: ")]
		public string? Mistnost { get; set; }
		
        [Required]
		[DisplayName("Řada: ")]
		public string? Rada { get; set; }

		[Required]
		[DisplayName("Regál: ")]
		public string? Regal { get; set; }

		[Required]
		[DisplayName("Pozice: ")]
		public string? Pozice { get; set; }

		[Required]
		[DisplayName("Datum: ")]
		public DateTime Datum { get; set; }

		[DisplayName("Poznámka: ")]
		public string? Poznamka { get; set; }
    }
}
