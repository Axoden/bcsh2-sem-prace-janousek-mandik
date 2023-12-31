using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Goods;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Management;

namespace sem_prace_janousek_mandik.Controllers.Management
{
    public class ManagementController : BaseController
	{
		/// <summary>
		/// Výpis všech pozic
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListPositions()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin)
			{
				List<Pozice> pozice = await ManagementSQL.GetAllPositions();
				return View(pozice);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech pozice
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchPositions(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin)
			{
				ViewBag.Search = search;
				List<Pozice> pozice = await ManagementSQL.GetAllPositions();
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
			if (Role == Roles.Admin)
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPosition(Pozice newPosition)
		{
			if (Role == Roles.Admin)
			{
				if (ModelState.IsValid)
				{
					if (await ManagementSQL.RegisterPosition(newPosition))
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
		public async Task<IActionResult> EditPositionGet(int index)
		{
			if (Role == Roles.Admin)
			{
				Pozice position = await ManagementSQL.GetPositionById(index);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPositionPost(Pozice position)
		{
			if (Role == Roles.Admin)
			{
				if (ModelState.IsValid)
				{
					await ManagementSQL.EditPosition(position);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePosition(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_POZICI", index);
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
			if (Role == Roles.Admin)
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
		public async Task<IActionResult> StartEmulationEmployee(int idEmployee)
		{
			if (Role == Roles.Admin)
			{
				Zamestnanci_Pozice employee = await EmployeeSQL.GetEmployeeRoleEmailById(idEmployee);
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
		public async Task<IActionResult> ListDatabaseObjects()
		{
			if (Role == Roles.Admin)
			{
				ViewBag.Tables = await ManagementSQL.GetAllObjects("table_name", "user_tables");
				ViewBag.Views = await ManagementSQL.GetAllObjects("view_name", "user_views");
				ViewBag.Indexes = await ManagementSQL.GetAllObjects("index_name", "user_indexes");
				ViewBag.Packages = await ManagementSQL.GetAllPackages();
				ViewBag.Procedures = await ManagementSQL.GetAllProceduresFunctions(true);
				ViewBag.Functions = await ManagementSQL.GetAllProceduresFunctions(false);
				ViewBag.Triggers = await ManagementSQL.GetAllObjects("trigger_name", "user_triggers");
				ViewBag.Sequences = await ManagementSQL.GetAllObjects("sequence_name", "user_sequences");

				return View();
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech databázových objektů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchDatabaseObjects(string search)
		{
			if (Role == Roles.Admin)
			{
				ViewBag.Search = search;
				List<string> tables = await ManagementSQL.GetAllObjects("table_name", "user_tables");
				List<string> views = await ManagementSQL.GetAllObjects("view_name", "user_views");
				List<string> indexes = await ManagementSQL.GetAllObjects("index_name", "user_indexes");
				List<string> packages = await ManagementSQL.GetAllPackages();
				List<string> procedures = await ManagementSQL.GetAllProceduresFunctions(true);
				List<string> functions = await ManagementSQL.GetAllProceduresFunctions(false);
				List<string> triggers = await ManagementSQL.GetAllObjects("trigger_name", "user_triggers");
				List<string> sequences = await ManagementSQL.GetAllObjects("sequence_name", "user_sequences");
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
		public async Task<IActionResult> ListLogs()
		{
			if (Role == Roles.Admin)
			{
				List<LogTableInsUpdDel> logs = await ManagementSQL.GetAllLogs();
				return View(logs);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech logů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchLogs(string search)
		{
			if (Role == Roles.Admin)
			{
				ViewBag.Search = search;
				List<LogTableInsUpdDel> logs = await ManagementSQL.GetAllLogs();
				if (search != null)
				{
					logs = logs.Where(lmb => (lmb?.TableName?.ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.Operation?.ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.ChangeTime.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.Username?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.OldData?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower()) || (lmb?.NewData?.ToString().ToLower() ?? string.Empty).Contains(search.ToLower())).ToList();
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
		public async Task<IActionResult> ListReports()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer)
			{
				Sestavy reports = await ManagementSQL.GetAllReports();
				return View(reports);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše přehledy (funkce)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> ListOverView()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer)
			{
				OverView overView = new();
				overView.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
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
		public async Task<IActionResult> ListOverViewCus(int idZakaznika)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer)
			{
				float customerValue = ManagementSQL.ListOverViewCus(idZakaznika);
				ViewBag.CustomerValue = customerValue;

				OverView overView = new();
				overView.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
				return View("ListOverView", overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda provede funkci největšího dodavatele
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> ListOverViewSuppliers()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer)
			{
				string? supplierValue = await ManagementSQL.ListOverViewSuppliers();
				ViewBag.SupplierValue = supplierValue;

				OverView overView = new();
				overView.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
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
		public async Task<IActionResult> ListOverViewCategories(int idKategorie)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer)
			{
				int idGoods = await ManagementSQL.ListOverViewCategories(idKategorie);
				if (idGoods != 0)
				{
					Zbozi zbozi = await GoodsSQL.GetGoodsById(idGoods);
					ViewBag.MaxGoods = zbozi.Nazev;
				}
				else
				{
					ViewBag.MaxGoods = "chyba";
				}

				OverView overView = new();
				overView.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
				overView.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
				return View("ListOverView", overView);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vypíše seznam všech souborů
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListFiles()
		{
			if (Role == Roles.Admin)
			{
				List<Soubory_Vypis> soubory = await ManagementSQL.GetAllFiles();
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
		public async Task<IActionResult> SearchFiles(string search)
		{
			if (Role == Roles.Admin)
			{
				ViewBag.Search = search;
				List<Soubory_Vypis> soubory = await ManagementSQL.GetAllFiles();
				if (search != null)
				{
					search = search.ToLower();
					soubory = soubory.Where(lmb => (lmb.Soubory?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Soubory?.TypSouboru ?? string.Empty).ToLower().Contains(search) || (lmb.Soubory?.PriponaSouboru != null && lmb.Soubory.PriponaSouboru.ToString().ToLower().Contains(search)) || (lmb.Soubory?.DatumNahrani != null && lmb.Soubory.DatumNahrani.ToString().ToLower().Contains(search)) || (lmb.Soubory?.DatumModifikace != null && lmb.Soubory.DatumModifikace.ToString().ToLower().Contains(search)) || (lmb.JmenoZamestnance ?? string.Empty).ToLower().Contains(search) || (lmb.PrijmeniZamestnance ?? string.Empty).ToLower().Contains(search) || (lmb.KdePouzito ?? string.Empty).ToLower().Contains(search)).ToList();
				}
				return View(nameof(ListFiles), soubory);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		/// <summary>
		/// Metoda na načtení formuláře na úpravu Souboru
		/// </summary>
		/// <param name="idSouboru"></param>
		/// <returns></returns>
		public async Task<IActionResult> EditFileGet(int idSouboru)
		{
			if (Role == Roles.Admin)
			{
				Soubory_Edit fileEdit = new();
				fileEdit.Soubory = await ManagementSQL.GetFileById(idSouboru);
				fileEdit.Zamestnanci = await EmployeeSQL.GetAllEmployeesNameSurname();
				return View("EditFile", fileEdit);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		/// <summary>
		/// Metoda přijme a upraví upravená data
		/// </summary>
		/// <param name="fileEdit">Model s upravenými daty souboru</param>
		/// <param name="soubor">Soubor</param>
		/// <returns></returns>
		public async Task<IActionResult> EditFilePost(Soubory_Edit fileEdit, IFormFile soubor)
		{
			if (Role == Roles.Admin)
			{
				fileEdit.Zamestnanci = await EmployeeSQL.GetAllEmployeesNameSurname();
				ModelState.Remove("soubor");
				if (ModelState.IsValid)
				{
					if (fileEdit.Soubory.DatumNahrani > DateTime.Now && fileEdit.Soubory.DatumModifikace > DateTime.Now)
					{
						ViewBag.ErrorInfo = "Datum nahrání nebo úpravy nesmí být v budoucnosti!";
						return View("EditFile", fileEdit);
					}

					if (fileEdit.Soubory != null)
					{
						if (soubor != null && soubor.Length > 0)
						{
							using (var memoryStream = new MemoryStream())
							{
								soubor.CopyTo(memoryStream);
								fileEdit.Soubory.Data = memoryStream.ToArray();
							}
							await ManagementSQL.EditFile(fileEdit);
						}
						await ManagementSQL.EditFile(fileEdit);
					}
					return RedirectToAction(nameof(ListFiles), nameof(Management));
				}
				return View("EditFile", fileEdit);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}

		/// <summary>
		/// Metoda zavolá proceduru na odstranění souboru
		/// </summary>
		/// <param name="index">ID souboru</param>
		/// <returns></returns>
		public async Task<IActionResult> DeleteFile(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("P_SMAZAT_SOUBOR", index);
			}
			return RedirectToAction(nameof(ListFiles), nameof(Management));
		}
	}
}