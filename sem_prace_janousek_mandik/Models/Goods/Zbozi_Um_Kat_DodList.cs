using sem_prace_janousek_mandik.Models.Supplier;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi_Um_Kat_DodList
    {
		public Zbozi? Zbozi { get; set; }

		public List<Umisteni>? Umisteni { get; set; }

		public List<Kategorie>? Kategorie { get; set; }

		public List<Dodavatele>? Dodavatele { get; set; }
	}
}