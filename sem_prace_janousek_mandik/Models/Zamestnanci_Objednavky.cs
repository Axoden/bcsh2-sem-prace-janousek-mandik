using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models
{
	public class Zamestnanci_Objednavky
	{
		[Browsable(false)]
		public int IdZamestnance { get; set; }

		[DisplayName("Jméno: ")]
		public string? Jmeno { get; set; }

		[DisplayName("Příjmení: ")]
		public string? Prijmeni { get; set; }

		[DisplayName("Název pozice: ")]
		public string? NazevPozice { get; set; }

		[DisplayName("Adresa zaměstnance: ")]
		public string? Adresa { get; set; }

		[DisplayName("Počet vyřízených objenávek: ")]
		public int PocetObjednavek { get; set; }
	}
}