using sem_prace_janousek_mandik.Models.Management;
using sem_prace_janousek_mandik.Models.Supplier;

namespace sem_prace_janousek_mandik.Models.Goods
{
    public class Zbozi_Um_Kat_Dod_Soubory
	{
		public Zbozi? Zbozi { get; set; }

		public Umisteni? Umisteni { get; set; }

		public Kategorie? Kategorie { get; set; }

		public Dodavatele? Dodavatele { get; set; }

        public Soubory? Soubory { get; set; }
    }
}