using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace sem_prace_janousek_mandik.Controllers
{
	public class BaseController : Controller
	{
		public string? Role { get; private set; }

		public string? Email { get; private set; }

		public string? EmulatedEmail { get; private set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			Role = HttpContext.Session.GetString("role") ?? "";
			Email = HttpContext.Session.GetString("email") ?? "";
			EmulatedEmail = HttpContext.Session.GetString("emulatedEmail") ?? "";

			ViewBag.Role = Role;
			ViewBag.Email = Email;
			ViewBag.EmulatedEmail = EmulatedEmail;
		}
	}
}
