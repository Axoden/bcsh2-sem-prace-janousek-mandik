using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_Zamestnanci_Zakaznici
	{
        public Objednavky? Objednavky { get; set; }

        public Zamestnanci? Zamestnanci { get; set; }

        public Zakaznici? Zakaznici { get; set; }
    }
}
