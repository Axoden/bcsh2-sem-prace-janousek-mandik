using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_Faktury_ZakList
	{
        public Objednavky? Objednavky { get; set; }

        public Faktury? Faktury { get; set; }

        public List<Zakaznici>? Zakaznici { get; set; }
    }
}
