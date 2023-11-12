using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_Zamestnanci_Faktury
	{
        public Objednavky? Objednavky { get; set; }

        public Zamestnanci? Zamestnanci { get; set; }

        public Faktury? Faktury { get; set; }
    }
}
