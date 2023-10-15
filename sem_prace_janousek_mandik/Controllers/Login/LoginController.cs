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

        // Nacteni stranky pro neprihlaseneho zamestnance
        [HttpGet]
        public IActionResult LoginEmployee()
        {
            return View();
        }

        // Prijem dat z prihlasovaciho formulare
        [HttpPost]
        public IActionResult LoginEmployee(Zamestnanci inputZamestnanec)
        {
            // Informativni zprava pokud neni nic vyplneno/je spatne vyplneno
            ViewBag.ErrorInfo = "Prihlasovaci jmeno nebo heslo je spatne!";
            if (inputZamestnanec != null)
            {
                Zamestnanci? dbZamestnanec = LoginSQL.AuthEmployee(inputZamestnanec.Email);

                if (dbZamestnanec != null)
                {
                    // Kontrola hashe hesel
                    if (dbZamestnanec.Heslo.Equals(HashPassword(inputZamestnanec.Heslo)))
                    {
                        HttpContext.Session.SetString("email", dbZamestnanec.Email);
                        Pozice? pozice = LoginSQL.GetPosition(dbZamestnanec.IdPozice);
                        if(pozice != null)
                        {
                            HttpContext.Session.SetString("role", pozice.Nazev);
                        }
                        else
                        {
                            HttpContext.Session.SetString("role", "noRole");
                        }
                        
                        return RedirectToAction("Index", "Home"); // redirect pri uspesnem prihlaseni
                    }
                }
            }

            return View(inputZamestnanec);
        }

        // Nacteni stranky pro neprihlaseneho zamestnance
        [HttpGet]
        public IActionResult LoginCustomer()
        {
            return View();
        }

        // Prijem dat z prihlasovaciho formulare
        [HttpPost]
        public IActionResult LoginCustomer(Zakaznici inputZakaznik)
        {
            // Informativni zprava pokud neni nic vyplneno/je spatne vyplneno
            ViewBag.ErrorInfo = "Prihlasovaci jmeno nebo heslo je spatne!";
            if (inputZakaznik != null)
            {
                Zakaznici? dbZakaznik = LoginSQL.AuthCustomer(inputZakaznik.Email);

                if (dbZakaznik != null)
                {
                    // Kontrola hashe hesel
                    if (dbZakaznik.Heslo.Equals(HashPassword(inputZakaznik.Heslo)))
                    {
                        HttpContext.Session.SetString("email", inputZakaznik.Email);
                        HttpContext.Session.SetString("role", "Zakaznik");
                        return RedirectToAction("Index", "Home"); // redirect pri uspesnem prihlaseni
                    }
                }
            }

            return View(inputZakaznik);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

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
