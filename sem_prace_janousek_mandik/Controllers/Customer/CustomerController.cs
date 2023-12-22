using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
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
		/// Příjem dat z přihlašovacího formuláře zákazníka
		/// </summary>
		/// <param name="inputZakaznik">Přihlašovací údaje</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginCustomer(ZamestnanciLoginForm inputZakaznik)
		{
			ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
			if (ModelState.IsValid == true)
			{
				Zakaznici? dbZakaznik = await CustomerSQL.AuthCustomer(inputZakaznik.Email);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterCustomer(Zakaznici_Adresy inputZakaznik)
		{
			ViewBag.ErrorInfo = "Některá pole nejsou správně vyplněna!";
			if (ModelState.IsValid)
			{
				inputZakaznik.Zakaznici.Heslo = SharedSQL.HashPassword(inputZakaznik.Zakaznici.Heslo);
				string? err = await CustomerSQL.RegisterCustomer(inputZakaznik);

				if (err == null)
				{
					if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
					{
						return RedirectToAction(nameof(ListCustomers), nameof(Customer));
					}
					// Úspěšná registrace, přesměrování na přihlášení
					return RedirectToAction(nameof(RegisterSuccessful), nameof(Customer));
				}
				else
				{
					ViewBag.ErrorInfo = err;
					return View(inputZakaznik);
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
		public async Task<IActionResult> EditCustomerGet(int index)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Zakaznici_Adresy zakazniciAdresy = await CustomerSQL.GetCustomerWithAddress(index);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditCustomerPost(Zakaznici_Adresy customer)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				// Nevyžadovat validaci, když se heslo nemění
				ModelState.Remove($"{nameof(Zakaznici_Adresy.Zakaznici)}.{nameof(Zakaznici.Heslo)}");
				ModelState.Remove($"{nameof(Zakaznici_Adresy.Zakaznici)}.{nameof(Zakaznici.HesloZnova)}");
				if (ModelState.IsValid)
				{
					if (customer.Zakaznici.Heslo != null)
					{
						if(customer.Zakaznici.Heslo.Length < 6)
						{
							ViewBag.ErrorInfo = "Délka hesla musí být minimálně 6 znaků";
							return View("EditCustomer", customer);
						}

						if (!customer.Zakaznici.Heslo.Equals(customer.Zakaznici.HesloZnova))
						{
							ViewBag.ErrorInfo = "Hesla se neshodují";
							return View("EditCustomer", customer);
						}
						customer.Zakaznici.Heslo = SharedSQL.HashPassword(customer.Zakaznici.Heslo);
					}

					string? err = await CustomerSQL.EditCustomer(customer);

					if (err == null)
					{
						return RedirectToAction(nameof(ListCustomers), nameof(Customer));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return View("EditCustomer", customer);
					}
				}
				return View("EditCustomer", customer);
			}
			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}

		/// <summary>
		/// Výpis všech zákazníků
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListCustomers()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				List<Zakaznici_Adresy> zakazniciAdresy = await CustomerSQL.GetAllCustomersWithAddresses();
				return View(zakazniciAdresy);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech zákazníků
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchCustomers(string search)
		{
			if (Role == Roles.Admin)
			{
				ViewBag.Search = search;
				List<Zakaznici_Adresy> customers = await CustomerSQL.GetAllCustomersWithAddresses();
				if (search != null)
				{
					search = search.ToLower();
					customers = customers.Where(lmb => (lmb.Zakaznici?.Jmeno ?? string.Empty).ToLower().Contains(search) || (lmb.Zakaznici?.Prijmeni ?? string.Empty).ToLower().Contains(search) || (lmb.Zakaznici?.Telefon ?? string.Empty).ToLower().Contains(search) || (lmb.Zakaznici?.Email ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Ulice ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Mesto ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Okres ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Zeme ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Psc ?? string.Empty).ToLower().Contains(search)).ToList();
				}
				return View(nameof(ListCustomers), customers);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Výpis evidovaných informací o zákazníkovi přihlášenému zákazníkovi
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListCustomer()
		{
			if (Role == Roles.Zakaznik)
			{
				Zakaznici_Adresy customerAddress = await CustomerSQL.GetCustomerWithAddressByEmail(Email);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteCustomer(int idZakaznika, int idAdresy)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_ZAKAZNIKA", idZakaznika, idAdresy);
			}
			return RedirectToAction(nameof(ListCustomers), nameof(Customer));
		}
	}
}
