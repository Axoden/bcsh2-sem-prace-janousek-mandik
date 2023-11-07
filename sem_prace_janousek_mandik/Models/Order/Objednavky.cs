namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky
	{
        public int IdObjednavky { get; set; }

		public int CisloObjednavky { get; set; }

		public DateTime DatumPrijeti { get; set; }

        public string? Poznamka { get; set; }

        public bool Uzavrena { get; set; }

        public int IdFaktury { get; set; }

        public int IdZakaznika { get; set; }

        public int IdZamestnance { get; set; }
    }
}