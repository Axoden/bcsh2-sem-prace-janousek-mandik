namespace sem_prace_janousek_mandik.Models.Payment
{
	public class Platba_Faktury
	{
        public PlatbyForm? Platby { get; set; }

        public List<Faktury>? Faktury { get; set; }
    }
}
