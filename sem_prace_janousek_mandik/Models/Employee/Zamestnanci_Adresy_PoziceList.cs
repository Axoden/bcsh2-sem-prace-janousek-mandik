using sem_prace_janousek_mandik.Models.Management;
using sem_prace_janousek_mandik.Models.Shared;

namespace sem_prace_janousek_mandik.Models.Employee
{
    public class Zamestnanci_Adresy_PoziceList
    {
        public Zamestnanci? Zamestnanci { get; set; }

        public Adresy? Adresy { get; set; }

        public List<Pozice>? Pozice { get; set; }
    }
}
