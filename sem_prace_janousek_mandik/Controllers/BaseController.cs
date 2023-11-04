using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace sem_prace_janousek_mandik.Controllers
{
	public class BaseController : Controller
	{
		public string? Role { get; private set; }
		public string? Email { get; private set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			Role = HttpContext.Session.GetString("role") ?? "";
			Email = HttpContext.Session.GetString("email") ?? "";

			ViewBag.Role = Role;
			ViewBag.Email = Email;
		}
	}
}
