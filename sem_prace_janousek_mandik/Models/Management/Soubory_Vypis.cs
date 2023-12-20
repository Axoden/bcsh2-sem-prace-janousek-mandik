using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Management
{
    public class Soubory_Vypis
    {
        public Soubory? Soubory { get; set; }

        [DisplayName("Jméno")]
        public string? JmenoZamestnance { get; set; }

        [DisplayName("a Příjmení Zaměstnance:")]
        public string? PrijmeniZamestnance { get; set; }

        [DisplayName("Je používán u zboží s názvem:")]
        public string? KdePouzito { get; set; }
    }
}