using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using System.Security.Cryptography;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Login
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        // Načtení přihlašovacího formuláře pro zaměstnance
        [HttpGet]
        public IActionResult LoginEmployee()
        {
            return View();
        }

        // Příjem dat z přihlašovacího formuláře pro zaměstnance
        [HttpPost]
        public IActionResult LoginEmployee(ZamestnanciLoginForm inputZamestnanec)
        {
            // Informativní zpráva při chybném vyplnění
            ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
            
            if (ModelState.IsValid)
            {
                Zamestnanci? dbZamestnanec = LoginSQL.AuthEmployee(inputZamestnanec.Email);

                if (dbZamestnanec != null)
                {
                    // Kontrola hashe hesel
                    if (dbZamestnanec.Heslo.Equals(HashPassword(inputZamestnanec.Heslo)))
                    {
                        // Nastavení session
                        HttpContext.Session.SetString("email", dbZamestnanec.Email);
                        Pozice? pozice = LoginSQL.GetPosition(dbZamestnanec.IdPozice);
                        if (pozice != null)
                        {
                            HttpContext.Session.SetString("role", pozice.Nazev);
                        }
                        else
                        {
                            HttpContext.Session.SetString("role", "Norole");
                        }

                        // Přesměrování při úspěšném přihlášení
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(inputZamestnanec);
        }

        // Načtení přihlašovacího formuláře pro zákazníky
        [HttpGet]
        public IActionResult LoginCustomer()
        {
            return View();
        }

        // Příjem dat z přihlašovacího formuláře zákazníka
        [HttpPost]
        public IActionResult LoginCustomer(Zakaznici inputZakaznik)
        {
            // Informativní zpráva při chybném vyplnění
            ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
            if (ModelState.IsValid == true)
            {
                Zakaznici? dbZakaznik = LoginSQL.AuthCustomer(inputZakaznik.Email);

                if (dbZakaznik != null)
                {
                    // Kontrola hashe hesel
                    if (dbZakaznik.Heslo.Equals(HashPassword(inputZakaznik.Heslo)))
                    {
                        HttpContext.Session.SetString("email", inputZakaznik.Email);
                        HttpContext.Session.SetString("role", "Zakaznik");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(inputZakaznik);
        }

        // Odhlášení a odstranění session
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Načtení registračního formuláře
        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        // Příjem dat z registrační formuláře zákazníka
        [HttpPost]
        public IActionResult RegisterCustomer(Zakaznici_Adresy inputZakaznik)
        {
            // Informativní zpráva při chybném vyplnění
            ViewBag.ErrorInfo = "Některá pole nejsou správně vyplněna!";

            if (ModelState.IsValid == true)
            {
                // Kontrola zda již není zaregistrován zákazník s tímto emailem
                if (LoginSQL.CheckExistsCustomer(inputZakaznik.Zakaznici.Email) == true)
                {
                    ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
                    return View(inputZakaznik);
                }

                bool uspesnaRegistrace = LoginSQL.RegisterCustomer(inputZakaznik);

                if (uspesnaRegistrace == true)
                {
                    // Úspěšná registrace, přesměrování na přihlášení
                    return RedirectToAction("RegisterSuccessful", "Login");
                }
            }
            return View(inputZakaznik);
        }

        // Informativní stránka po úspěšné registraci
        public IActionResult RegisterSuccessful()
        {
            return View();
        }

        // Vygenerování hashe z hesla
        public static string? HashPassword(string password)
        {
            if (password != null)
            {
                var sha = SHA256.Create();
                var encoded = Encoding.Default.GetBytes(password);
                var hashedPassword = sha.ComputeHash(encoded);
                return Convert.ToBase64String(hashedPassword);
            }
            return null;
        }
    }
}