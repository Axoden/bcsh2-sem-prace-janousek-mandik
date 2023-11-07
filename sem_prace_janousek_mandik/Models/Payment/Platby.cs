namespace sem_prace_janousek_mandik.Models.Payment
{
	public class Platby
	{
        public int IdPlatby { get; set; }

        public DateTime DatumPlatby { get; set; }

        public float Castka { get; set; }

        public char TypPlatby { get; set; }

        public string? VariabilniSymbol { get; set; }

        public int IdFaktury { get; set; }
    }
}
