namespace sem_prace_janousek_mandik.Models.Order
{
	public class ZboziObjednavek
	{
        public int IdZboziObjednavky { get; set; }

        public int Mnozstvi { get; set; }

        public float JednotkovaCena { get; set; }

        public int IdObjednavky { get; set; }

        public int IdZbozi { get; set; }
    }
}
