using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_Zakaznici_Faktury
	{
        public Objednavky? Objednavky { get; set; }

        public Zakaznici? Zakaznici { get; set; }

        public Faktury? Faktury { get; set; }
    }
}
