using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using sem_prace_janousek_mandik.Controllers.Management;

namespace sem_prace_janousek_mandik.Controllers
{
    public class BaseController : Controller
    {
        public Roles? Role { get; private set; }

        public string? Email { get; private set; }

        public string? EmulatedEmail { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string sessionRole = HttpContext.Session.GetString("role") ?? "";
            Role = GetRoleFromString(sessionRole);
            Email = HttpContext.Session.GetString("email") ?? "";
            EmulatedEmail = HttpContext.Session.GetString("emulatedEmail") ?? "";

            ViewBag.Role = Role;
            ViewBag.Email = Email;
            ViewBag.EmulatedEmail = EmulatedEmail;
        }

        private static Roles? GetRoleFromString(string stringRole)
        {
            foreach (Roles item in Enum.GetValues(typeof(Roles)))
            {
                if (item.ToString().Equals(stringRole))
                {
                    return item;
                }
            }
            return null;
        }
    }
}