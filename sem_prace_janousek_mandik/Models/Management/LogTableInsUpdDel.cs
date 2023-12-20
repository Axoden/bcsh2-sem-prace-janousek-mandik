namespace sem_prace_janousek_mandik.Models.Management
{
    public class LogTableInsUpdDel
    {
        public int LogId { get; set; }

        public string? TableName { get; set; }

        public string? Operation { get; set; }

        public DateTime ChangeTime { get; set; }

        public string? Username { get; set; }

        public string? OldData { get; set; }

        public string? NewData { get; set; }
    }
}