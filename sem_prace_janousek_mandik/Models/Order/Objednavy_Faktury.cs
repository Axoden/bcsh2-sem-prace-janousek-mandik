using sem_prace_janousek_mandik.Models.Payment;

namespace sem_prace_janousek_mandik.Models.Order
{
    public class Objednavy_Faktury
    {
        public Objednavky? Objednavky { get; set; }

        public Faktury? Faktury { get; set; }
    }
}
