using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Umisteni
	{
        [Browsable(false)]
        public int IdUmisteni { get; set; }

        [Required]
		[RegularExpression("^[0-9]", ErrorMessage = "Musí obsahovat jednu číslici.")]
		[DisplayName("Místnost: ")]
		public string? Mistnost { get; set; }
		
        [Required]
		[RegularExpression("^[A-Z]$", ErrorMessage = "Musí obsahovat jedno velké písmeno.")]
		[DisplayName("Řada: ")]
		public string? Rada { get; set; }

		[Required]
		[RegularExpression("^[0-9]{2}$", ErrorMessage = "Musí obsahovat dvě číslice.")]
		[DisplayName("Regál: ")]
		public string? Regal { get; set; }

		[Required]
		[RegularExpression("^_[0-9]$", ErrorMessage = "Musí obsahovat podtržítko (_) a jednu číslici.")]
		[DisplayName("Pozice: ")]
		public string? Pozice { get; set; }

		[Required]
		[DisplayName("Poslední změna na umístění: ")]
		public DateTime Datum { get; set; }

		[DisplayName("Poznámka: ")]
		public string? Poznamka { get; set; }
    }
}
