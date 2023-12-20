using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Employee
{
    public class ZamestnanciLoginForm
    {
        [Required]
        [EmailAddress]
		[DisplayName("Email: ")]
		public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
		[DisplayName("Heslo: ")]
		public string? Heslo { get; set; }
    }
}
