using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
	public class Objednavky_List
	{
        public List<Objednavky_Zamestnanci_Zakaznici_Faktury>? Objednavky_Zam_Zak_Fak { get; set; }

        public List<ZboziObjednavek_Zbozi>? ZboziObjednavek_Zbozi { get; set; }

		public List<Platby>? Platby { get; set; }
	}
}
