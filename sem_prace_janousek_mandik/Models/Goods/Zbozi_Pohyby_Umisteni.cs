namespace sem_prace_janousek_mandik.Models.Goods
{
	public class Zbozi_Pohyby_Umisteni
	{
        public List<Zbozi>? Zbozi { get; set; }

        public List<Movements>? Movements { get; set; }

        public List<Umisteni>? Umisteni { get; set; }
    }
}
