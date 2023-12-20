using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models.Goods
{
    public class Zbozi_Sklad
    {
        [Browsable(false)]
        public int IdZbozi { get; set; }

        [DisplayName("Název: ")]
        public string? Nazev { get; set; }

        [DisplayName("Jednotková cena: ")]
        public float? JednotkovaCena { get; set; }

        [DisplayName("Počet na skladě: ")]
        public int PocetNaSklade { get; set; }

        [DisplayName("Název kategorie: ")]
        public string? NazevKategorie { get; set; }

        [DisplayName("Nadřazená kategorie: ")]
        public string? NadrazenaKategorie { get; set; }

        [DisplayName("Název dodavatele: ")]
        public string? NazevDodavatele { get; set; }

        [DisplayName("Umístění: ")]
        public string? Umisteni { get; set; }
    }
}