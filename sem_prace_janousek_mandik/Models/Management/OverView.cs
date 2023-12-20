using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Goods;

namespace sem_prace_janousek_mandik.Models.Management
{
    public class OverView
    {
        public List<Zakaznici>? Zakaznici { get; set; }

        public List<Kategorie>? Kategorie { get; set; }
    }
}