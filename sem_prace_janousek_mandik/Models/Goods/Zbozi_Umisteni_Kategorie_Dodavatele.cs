using sem_prace_janousek_mandik.Models.Supplier;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi_Umisteni_Kategorie_Dodavatele
	{
		public Zbozi? Zbozi { get; set; }

		public Umisteni? Umisteni { get; set; }

		public Kategorie? Kategorie { get; set; }

		public Dodavatele? Dodavatele { get; set; }
	}
}