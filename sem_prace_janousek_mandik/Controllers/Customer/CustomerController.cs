using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;

namespace sem_prace_janousek_mandik.Controllers.Customer
{
	public class CustomerController : BaseController
	{
		// Načtení přihlašovacího formuláře pro zákazníky
		[HttpGet]
		public IActionResult LoginCustomer()
		{
			return View();
		}

		// Příjem dat z přihlašovacího formuláře zákazníka
		[HttpPost]
		public IActionResult LoginCustomer(ZamestnanciLoginForm inputZakaznik)
		{
			// Informativní zpráva při chybném vyplnění
			ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
			if (ModelState.IsValid == true)
			{
				Zakaznici? dbZakaznik = CustomerSQL.AuthCustomer(inputZakaznik.Email);

				if (dbZakaznik != null)
				{
					// Kontrola hashe hesel
					if (dbZakaznik.Heslo.Equals(SharedSQL.HashPassword(inputZakaznik.Heslo)))
					{
						HttpContext.Session.SetString("email", inputZakaznik.Email);
						HttpContext.Session.SetString("role", "Zakaznik");
						return RedirectToAction("Index", "Home");
					}
				}
			}
			return View(inputZakaznik);
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
				if (CustomerSQL.CheckExistsCustomer(inputZakaznik.Zakaznici.Email) == true)
				{
					ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
					return View(inputZakaznik);
				}

				Zakaznici_Adresy inputZakaznikHashed = inputZakaznik;
				inputZakaznikHashed.Zakaznici.Heslo = SharedSQL.HashPassword(inputZakaznik.Zakaznici.Heslo);
				bool uspesnaRegistrace = CustomerSQL.RegisterCustomer(inputZakaznikHashed);

				if (uspesnaRegistrace == true)
				{
					// Úspěšná registrace, přesměrování na přihlášení
					return RedirectToAction(nameof(RegisterCustomer), nameof(Customer));
				}
			}
			return View(inputZakaznik);
		}

		// Informativní stránka po úspěšné registraci
		public IActionResult RegisterSuccessful()
		{
			return View();
		}

		// Načtení formuláře na úpravu vybraného zákazníka
		[HttpGet]
		public IActionResult EditCustomer(int index)
		{
			// Kontrola oprávnění na načtení parametrů zaměstnance
			if (Role.Equals("Admin"))
			{
				Zakaznici_Adresy zakazniciAdresy = CustomerSQL.GetCustomerWithAddress(index);
				return View(zakazniciAdresy);
			}

			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}

		// Příjem upravených dat vybraného zákazníka
		[HttpPost]
		public IActionResult EditCustomer(Zakaznici_Adresy zakazniciAdresy, int idZakaznika, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				zakazniciAdresy.Zakaznici.IdZakaznika = idZakaznika;
				zakazniciAdresy.Zakaznici.IdAdresy = idAdresy;

				CustomerSQL.EditCustomer(zakazniciAdresy);
			}

			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}

		// Výpis všech zaměstnanců
		public IActionResult ListCustomers()
		{
			if (Role.Equals("Admin"))
			{
				List<Zakaznici_Adresy> zakazniciAdresy = CustomerSQL.GetAllCustomersWithAddresses();
				return View(zakazniciAdresy);
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Formální metoda pro odstranění vybraného zákazníka
		[HttpGet]
		public IActionResult DeleteCustomer(int idZakaznika, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZAKAZNIKA", idZakaznika, idAdresy);
			}

			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}
	}
}
