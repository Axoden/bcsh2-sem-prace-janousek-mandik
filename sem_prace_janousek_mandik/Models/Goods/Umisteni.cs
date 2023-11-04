using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Umisteni
	{
        [Browsable(false)]
        public int IdUmisteni { get; set; }

        [Required]
        public string? Mistnost { get; set; }
		
        [Required]
		public string? Rada { get; set; }

		[Required]
		public string? Regal { get; set; }

		[Required]
		public string? Pozice { get; set; }

		[Required]
		public DateTime Datum { get; set; }

        public string? Poznamka { get; set; }
    }
}
