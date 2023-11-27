using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Goods;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public class ManagementController : BaseController
	{
		/// <summary>
		/// Výpis všech pozic
		/// </summary>
		/// <returns></returns>
		public IActionResult ListPositions()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin"))
			{
				List<Pozice> pozice = ManagementSQL.GetAllPositions();
				return View(pozice);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech pozice
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchPositions(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin"))
			{
				ViewBag.Search = search;
				List<Pozice> pozice = ManagementSQL.GetAllPositions();
				if (search != null)
				{
					pozice = pozice.Where(lmb => (lmb.Nazev?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();

				}
				return View(nameof(ListPositions), pozice);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání nové pozice
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult AddPosition()
		{
			if (Role.Equals("Admin"))
			{
				return View();
			}
			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		/// <summary>
		/// Příjem dat z formuláře na přidání pozice
		/// </summary>
		/// <param name="newPosition">Model s daty nové pozice</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddPosition(Pozice newPosition)
		{
			if (Role.Equals("Admin"))
			{
				if (ModelState.IsValid == true)
				{
					if (ManagementSQL.RegisterPosition(newPosition))
					{
						return RedirectToAction(nameof(ListPositions), nameof(Management));
					}
				}
				return View(newPosition);
			}
			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybrané pozice
		/// </summary>
		/// <param name="index">ID upravované pozice</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditPositionGet(int index)
		{
			if (Role.Equals("Admin"))
			{
				Pozice position = ManagementSQL.GetPositionById(index);
				return View("EditPosition", position);
			}
			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		/// <summary>
		/// Příjem upravených dat vybrané pozice
		/// </summary>
		/// <param name="position">Model s upravenými daty pozice</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditPositionPost(Pozice position)
		{
			if (Role.Equals("Admin"))
			{
				if (ModelState.IsValid == true)
				{
					ManagementSQL.EditPosition(position);
				}
				else
				{
					return View("EditPosition", position);
				}
			}
			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané pozice
		/// </summary>
		/// <param name="index">ID pozice</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeletePosition(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_POZICI", index);
			}
			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		/// <summary>
		/// Metoda emuluje admina za vybraného zákazníka
		/// </summary>
		/// <param name="emailCustomer">email zákazníka</param>
		/// <returns></returns>
		public IActionResult StartEmulationCustomer(string emailCustomer)
		{
			if (Role.Equals("Admin"))
			{
				HttpContext.Session.SetString("emulatedEmail", Email);
				HttpContext.Session.SetString("email", emailCustomer);
				HttpContext.Session.SetString("role", "Zakaznik");
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda emuluje admina za vybraného zaměstnance
		/// </summary>
		/// <param name="idEmployee">ID zaměstnance</param>
		/// <returns></returns>
		public IActionResult StartEmulationEmployee(int idEmployee)
		{
			if (Role.Equals("Admin"))
			{
				Zamestnanci_Pozice employee = EmployeeSQL.GetEmployeeRoleEmailById(idEmployee);
				HttpContext.Session.SetString("emulatedEmail", Email);
				HttpContext.Session.SetString("email", employee.Zamestnanci.Email);
				HttpContext.Session.SetString("role", employee.Pozice.Nazev);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda ukončí emulaci a přepne zpět na původního admina
		/// </summary>
		/// <returns></returns>
		public IActionResult EndEmulation()
		{
			if (!EmulatedEmail.Equals(""))
			{
				string adminEmail = EmulatedEmail;
				HttpContext.Session.SetString("emulatedEmail", "");
				HttpContext.Session.SetString("email", adminEmail);
				HttpContext.Session.SetString("role", "Admin");
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše všechny databázové objekty
		/// </summary>
		/// <returns></returns>
		public IActionResult ListDatabaseObjects()
		{
			if (Role.Equals("Admin"))
			{
				ViewBag.Tables = ManagementSQL.GetAllObjects("table_name", "user_tables");
				ViewBag.Views = ManagementSQL.GetAllObjects("view_name", "user_views");
				ViewBag.Indexes = ManagementSQL.GetAllObjects("index_name", "user_indexes");
				ViewBag.Packages = ManagementSQL.GetAllPackages();
				ViewBag.Procedures = ManagementSQL.GetAllProceduresFunctions(true);
				ViewBag.Functions = ManagementSQL.GetAllProceduresFunctions(false);
				ViewBag.Triggers = ManagementSQL.GetAllObjects("trigger_name", "user_triggers");
				ViewBag.Sequences = ManagementSQL.GetAllObjects("sequence_name", "user_sequences");

				return View();
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech databázových objektů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchDatabaseObjects(string search)
		{
			if (Role.Equals("Admin"))
			{
				ViewBag.Search = search;
				List<string> tables = ManagementSQL.GetAllObjects("table_name", "user_tables");
				List<string> views = ManagementSQL.GetAllObjects("view_name", "user_views");
				List<string> indexes = ManagementSQL.GetAllObjects("index_name", "user_indexes");
				List<string> packages = ManagementSQL.GetAllPackages();
				List<string> procedures = ManagementSQL.GetAllProceduresFunctions(true);
				List<string> functions = ManagementSQL.GetAllProceduresFunctions(false);
				List<string> triggers = ManagementSQL.GetAllObjects("trigger_name", "user_triggers");
				List<string> sequences = ManagementSQL.GetAllObjects("sequence_name", "user_sequences");
				if (search != null)
				{
					tables = tables.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					views = views.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					indexes = indexes.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					packages = packages.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					procedures = procedures.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					functions = functions.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					triggers = triggers.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
					sequences = sequences.Where(lmb => (lmb?.ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
				}

				ViewBag.Tables = tables;
				ViewBag.Views = views;
				ViewBag.Indexes = indexes;
				ViewBag.Packages = packages;
				ViewBag.Procedures = procedures;
				ViewBag.Functions = functions;
				ViewBag.Triggers = triggers;
				ViewBag.Sequences = sequences;
				return View("ListDatabaseObjects");
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše změny dat v tabulkách
		/// </summary>
		/// <returns></returns>
		public IActionResult ListLogs()
		{
			if (Role.Equals("Admin"))
			{
				List<LogTableInsUpdDel> logs = ManagementSQL.GetAllLogs();
				return View(logs);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech logů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchLogs(string search)
		{
			if (Role.Equals("Admin"))
			{
				ViewBag.Search = search;
				List<LogTableInsUpdDel> logs = ManagementSQL.GetAllLogs();
				if (search != null)
				{
					logs = logs.Where(lmb => (lmb?.TableName?.ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.Operation?.ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.ChangeTime?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.Username?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.OldData?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.NewData?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
				}
				return View(nameof(ListLogs), logs);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše všechny sestavy (pohledy)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ListReports()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer"))
			{
				Sestavy reports = ManagementSQL.GetAllReports(true);
				return View(reports);
			}

			if (Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				Sestavy reports = ManagementSQL.GetAllReports(false);
				return View(reports);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše přehledy (funkce)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ListOverView()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer"))
			{
				OverView overView = new();
				overView.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				return View(overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda provede funkci celkové hodnoty objednávek zákazníka
		/// </summary>
		/// <param name="idZakaznika">ID zákazníka</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult ListOverViewCus(int idZakaznika)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer"))
			{
				float customerValue = ManagementSQL.ListOverViewCus(idZakaznika);
				ViewBag.CustomerValue = customerValue;

				OverView overView = new();
				overView.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				return View("ListOverView", overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda provede funkci největšího dodavatele
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult ListOverViewSuppliers()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer"))
			{
				string supplierValue = ManagementSQL.ListOverViewSuppliers();
				ViewBag.SupplierValue = supplierValue;

				OverView overView = new();
				overView.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				return View("ListOverView", overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda provede funkci nejvíce objednaného zboží v kategorii
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult ListOverViewCategories(int idKategorie)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer"))
			{
				int idGoods = ManagementSQL.ListOverViewCategories(idKategorie);
				if (idGoods != 0)
				{
					Zbozi zbozi = GoodsSQL.GetGoodsById(idGoods);
					ViewBag.MaxGoods = zbozi.Nazev;
				}
				else
				{
					ViewBag.MaxGoods = "chyba";
				}

				OverView overView = new();
				overView.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				return View("ListOverView", overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše seznam všech souborů
		/// </summary>
		/// <returns></returns>
		public IActionResult ListFiles()
		{
			if (Role.Equals("Admin"))
			{
				List<Soubory_Vypis> soubory = ManagementSQL.GetAllFiles();
				return View(soubory);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda slouží k vyhledávání ve výpisu všech souborů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchFiles(string search)
		{
			if (Role.Equals("Admin"))
			{
				ViewBag.Search = search;
				List<Soubory_Vypis> soubory = ManagementSQL.GetAllFiles();
				if (search != null)
				{
					soubory = soubory.Where(lmb => lmb.Soubory.Nazev.ToLower().Contains(search.ToLower()) || lmb.Soubory.TypSouboru.ToLower().Contains(search.ToLower()) || lmb.Soubory.PriponaSouboru.ToString().ToLower().Contains(search.ToLower()) || lmb.Soubory.DatumNahrani.ToString().ToLower().Contains(search.ToLower()) || lmb.Soubory.DatumModifikace.ToString().ToLower().Contains(search.ToLower()) || lmb.JmenoZamestnance.ToLower().Contains(search.ToLower()) || lmb.PrijmeniZamestnance.ToLower().Contains(search.ToLower()) || lmb.KdePouzito.ToLower().Contains(search.ToLower())).ToList();
				}
				return View(nameof(ListFiles), soubory);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		public IActionResult EditFileGet(int idSouboru)
		{
			if (Role.Equals("Admin"))
			{
				Soubory_Edit fileEdit = new();
				fileEdit.Soubory = ManagementSQL.GetFileById(idSouboru);
				fileEdit.Zamestnanci = EmployeeSQL.GetAllEmployeesNameSurname();
				return View("EditFile", fileEdit);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		public IActionResult EditFilePost(Soubory_Edit fileEdit, IFormFile file)
		{
			if (Role.Equals("Admin"))
			{
				if (fileEdit.Soubory != null)
				{
					if (file != null && file.Length > 0)
					{
						using (var memoryStream = new MemoryStream())
						{
							file.CopyTo(memoryStream);
							fileEdit.Soubory.Data = memoryStream.ToArray();
						}
						ManagementSQL.EditFile(fileEdit);
					}
					ManagementSQL.EditFile(fileEdit);
				}
				else
				{
					return View("EditFile", fileEdit);
				}
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		public IActionResult DeleteFile(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_SOUBOR", index);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}
	}
}