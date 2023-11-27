using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_Zam_Zak_FakturyList
	{
		public Objednavky? Objednavky { get; set; }

		public List<Zamestnanci>? Zamestnanci { get; set; }

		public List<Zakaznici>? Zakaznici { get; set; }

        public List<Faktury>? Faktury { get; set; }
    }
}
