using sem_prace_janousek_mandik.Models.Goods;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class ZboziObjednavek_ZboziList
	{
        public ZboziObjednavek? ZboziObjednavek { get; set; }

        public List<Zbozi>? Zbozi { get; set; }

        public int IdObjednavky { get; set; }
    }
}
