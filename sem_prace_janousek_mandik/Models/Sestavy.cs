namespace sem_prace_janousek_mandik.Models
{
	public class Sestavy
	{
		public List<Faktury_Zak_Zam>? Faktury { get; set; }

		public List<Zamestnanci_Objednavky>? ZamestnanciObjednavky { get; set; }

		public List<Zbozi_Sklad>? ZboziSklad { get; set; }
	}
}