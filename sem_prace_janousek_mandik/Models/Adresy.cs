using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Adresy
    {
        [Browsable(false)]
        public int IdAdresy { get; set; }

        [Required]
        public string? Ulice {  get; set; }

        [Required]
        public string? Mesto { get; set; }

        public string? Okres { get; set; }

        [Required]
        public string? Zeme { get; set; }

        [Required]
        [Range(0, 99999)]
        public string? Psc { get; set; }
    }
}
