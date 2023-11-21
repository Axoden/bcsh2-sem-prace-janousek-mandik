using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;

namespace sem_prace_janousek_mandik.Controllers.Customer
{
	public class CustomerController : BaseController
	{
		/// <summary>
		/// Načtení přihlašovacího formuláře pro zákazníky
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult LoginCustomer()
		{
			return View();
		}

		/// <summary>
		// Příjem dat z přihlašovacího formuláře zákazníka
		/// </summary>
		/// <param name="inputZakaznik">Přihlašovací údaje</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult LoginCustomer(ZamestnanciLoginForm inputZakaznik)
		{
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
                        return RedirectToAction(nameof(HomeController.Index), nameof(Home));
                    }
				}
			}
			return View(inputZakaznik);
		}

		/// <summary>
		/// Načtení registračního formuláře
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult RegisterCustomer()
		{
			return View();
		}

		/// <summary>
		/// Příjem dat z registrační formuláře zákazníka	
		/// </summary>
		/// <param name="inputZakaznik">Registrační údaje zákazníka</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult RegisterCustomer(Zakaznici_Adresy inputZakaznik)
		{
			ViewBag.ErrorInfo = "Některá pole nejsou správně vyplněna!";
			if (ModelState.IsValid == true)
			{
				// Kontrola zda již není zaregistrován zákazník s tímto emailem
				if (CustomerSQL.CheckExistsCustomerEmail(inputZakaznik.Zakaznici.Email) == true)
				{
					ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
					return View(inputZakaznik);
				}

                // Kontrola zda již není zaregistrován zákazník s tímto telefonním číslem
                if (CustomerSQL.CheckExistsCustomerPhone(inputZakaznik.Zakaznici.Telefon) == true)
                {
                    ViewBag.ErrorInfo = "Toto telefonní číslo je již zaregistrováno!";
                    return View(inputZakaznik);
                }

                inputZakaznik.Zakaznici.Heslo = SharedSQL.HashPassword(inputZakaznik.Zakaznici.Heslo);

				if (CustomerSQL.RegisterCustomer(inputZakaznik))
				{
					if (Role.Equals("Admin"))
					{
						return RedirectToAction(nameof(ListCustomers), nameof(Customer));
					}
					// Úspěšná registrace, přesměrování na přihlášení
					return RedirectToAction(nameof(RegisterSuccessful), nameof(Customer));
				}
			}
			return View(inputZakaznik);
		}

		/// <summary>
		/// Informativní stránka po úspěšné registraci
		/// </summary>
		/// <returns></returns>
		public IActionResult RegisterSuccessful()
		{
			return View();
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybraného zákazníka
		/// </summary>
		/// <param name="index">ID upravovaného zákazníka</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditCustomerGet(int index)
		{
			if (Role.Equals("Admin"))
			{
				Zakaznici_Adresy zakazniciAdresy = CustomerSQL.GetCustomerWithAddress(index);
				return View("EditCustomer", zakazniciAdresy);
			}
			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}

		/// <summary>
		/// Příjem upravených dat vybraného zákazníka
		/// </summary>
		/// <param name="customer"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditCustomerPost(Zakaznici_Adresy customer)
		{
			if (Role.Equals("Admin"))
			{
				if(customer.Zakaznici.Heslo != null)
				{
                    customer.Zakaznici.Heslo = SharedSQL.HashPassword(customer.Zakaznici.Heslo);
                }

				if (!CustomerSQL.EditCustomer(customer))
				{
                    return View("EditCustomer", customer);
                }
			}
			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}

		/// <summary>
		/// Výpis všech zákazníků
		/// </summary>
		/// <returns></returns>
		public IActionResult ListCustomers()
		{
			if (Role.Equals("Admin"))
			{
				List<Zakaznici_Adresy> zakazniciAdresy = CustomerSQL.GetAllCustomersWithAddresses();
				return View(zakazniciAdresy);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech zákazníků
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchCustomers(string search)
		{
			if (Role.Equals("Admin"))
			{
				ViewBag.Search = search;
				List<Zakaznici_Adresy> customers = CustomerSQL.GetAllCustomersWithAddresses();
				if (search != null)
				{
					customers = customers.Where(lmb => lmb.Zakaznici.Jmeno.ToLower().Contains(search.ToLower()) || lmb.Zakaznici.Prijmeni.ToLower().Contains(search.ToLower()) || lmb.Zakaznici.Telefon.ToLower().Contains(search.ToLower()) || lmb.Zakaznici.Email.ToLower().Contains(search.ToLower()) || lmb.Adresy.Ulice.ToLower().Contains(search.ToLower()) || lmb.Adresy.Mesto.ToLower().Contains(search.ToLower()) || lmb.Adresy.Okres.ToLower().Contains(search.ToLower()) || lmb.Adresy.Zeme.ToLower().Contains(search.ToLower()) || lmb.Adresy.Psc.ToLower().Contains(search.ToLower())).ToList();
				}
				return View(nameof(ListCustomers), customers);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Výpis evidovaných informací o zákazníkovi přihlášenému zákazníkovi
		/// </summary>
		/// <returns></returns>
		public IActionResult ListCustomer()
		{
			if (Role.Equals("Zakaznik"))
			{
				Zakaznici_Adresy customerAddress = CustomerSQL.GetCustomerWithAddressByEmail(Email);
				return View(customerAddress);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zákazníka
		/// </summary>
		/// <param name="idZakaznika">ID odstraňovaného zákazníka</param>
		/// <param name="idAdresy">ID odstraňované adresy</param>
		/// <returns></returns>
		[HttpPost]
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
