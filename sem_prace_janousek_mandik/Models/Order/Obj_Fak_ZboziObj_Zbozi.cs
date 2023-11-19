using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
    public class Obj_Fak_ZboziObj_Zbozi
	{
        public Objednavky? Objednavky { get; set; }

        public Faktury? Faktury { get; set; }

        public ZboziObjednavek? ZboziObjednavek { get; set; }

        public Zbozi? Zbozi { get; set; }
    }
}
