using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Pozice
    {
        [Browsable(false)]
        public int IdPozice { get; set; }

        [Required]
        public string? Nazev { get; set; }
    }
}
