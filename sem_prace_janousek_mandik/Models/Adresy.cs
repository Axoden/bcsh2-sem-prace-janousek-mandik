using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models
{
    public class Adresy
    {
        [Browsable(false)]
        public int IdAdresy { get; set; }

        [Required]
        [DisplayName("Ulice: ")]
        public string? Ulice {  get; set; }

        [Required]
        [DisplayName("Město: ")]
        public string? Mesto { get; set; }

        [DisplayName("Okres: ")]
        public string? Okres { get; set; }

        [Required]
        [DisplayName("Země: ")]
        public string? Zeme { get; set; }

        [Required]
        [Range(0, 99999)]
        [DisplayName("PSČ: ")]
        public string? Psc { get; set; }
    }
}
