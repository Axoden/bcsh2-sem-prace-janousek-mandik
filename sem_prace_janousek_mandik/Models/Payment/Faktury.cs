using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Payment
{
	public class Faktury
	{
        public int IdFaktury { get; set; }

        public int CisloFaktury { get; set; }

        [DataType(DataType.Date)]
		public DateOnly? DatumVystaveni { get; set; }

		[DataType(DataType.Date)]
		public DateOnly? DatumSplatnosti { get; set; }

        public float CastkaObjednavka { get; set; }

        public float CastkaDoprava { get; set; }

        public float Dph { get; set; }
    }
}
