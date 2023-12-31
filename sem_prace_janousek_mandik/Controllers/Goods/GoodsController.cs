using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Controllers.Supplier;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Management;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
	public class GoodsController : BaseController
	{
		/// <summary>
		/// Výpis všech kategorií
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListCategories()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				List<Kategorie_NadrazenaKategorie> categories = await GoodsSQL.GetAllCategories();
				return View(categories);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech kategorií
		/// </summary>
		/// <param name="search">Vstup pro vyhledávání</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchCategories(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				ViewBag.Search = search;
				List<Kategorie_NadrazenaKategorie> categories = await GoodsSQL.GetAllCategories();
				if (search != null)
				{
					search = search.ToLower();
					categories = categories.Where(lmb => (lmb.Kategorie?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Kategorie?.Zkratka ?? string.Empty).ToLower().Contains(search) || (lmb.Kategorie?.Popis ?? string.Empty).ToLower().Contains(search) || (lmb.NadrazenaKategorie?.Nazev ?? string.Empty).ToLower().Contains(search)).ToList();
				}
				return View(nameof(ListCategories), categories);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání nové kategorie
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddCategory()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Kategorie_NadrazenaKategorie_List category = new();
				category.NadrazenaKategorie = await GoodsSQL.GetAllCategoriesIdNameAcronym();
				return View(category);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		/// <summary>
		/// Příjem dat z formuláře na přidání kategorie
		/// </summary>
		/// <param name="newCategory">Model s daty nové kategorie</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddCategory(Kategorie_NadrazenaKategorie_List newCategory)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				newCategory.NadrazenaKategorie = await GoodsSQL.GetAllCategoriesIdNameAcronym();
				if (ModelState.IsValid)
				{
					newCategory.Kategorie.Zkratka = newCategory.Kategorie.Zkratka.ToUpper();
					// Kontrola zda již neexistuje kategorie s touto zkratkou
					if (await GoodsSQL.CheckExistsAcronym(newCategory.Kategorie.Zkratka) == true)
					{
						ViewBag.ErrorInfo = "Tato zkratka je již používána!";
						return View(newCategory);
					}

					if (await GoodsSQL.AddCategory(newCategory.Kategorie))
					{
						return RedirectToAction(nameof(ListCategories), nameof(Goods));
					}
				}
				return View(newCategory);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybrané kategorie
		/// </summary>
		/// <param name="index">ID upravované kategorie</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditCategoryGet(int index)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Kategorie_NadrazenaKategorie_List category = new();
				category.Kategorie = await GoodsSQL.GetCategoryById(index);
				category.NadrazenaKategorie = await GoodsSQL.GetAllCategoriesIdNameAcronym();
				return View("EditCategory", category);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		/// <summary>		
		/// Příjem upravených dat vybrané kategorie
		/// </summary>
		/// <param name="category">Model s upravenými daty kategorie</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditCategoryPost(Kategorie_NadrazenaKategorie_List category)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				if (ModelState.IsValid)
				{
					category.Kategorie.Zkratka = category.Kategorie.Zkratka?.ToUpper();
					string? acronym = await GoodsSQL.GetAcronymByIdCategory(category.Kategorie.IdKategorie);
					// Kontrola zda již neexistuje kategorie s touto zkratkou
					if (!acronym.Equals(category.Kategorie.Zkratka))
					{
						if (await GoodsSQL.CheckExistsAcronym(category.Kategorie.Zkratka) == true)
						{
							ViewBag.ErrorInfo = "Tato zkratka je již používána!";
							return await ReturnBad();
						}
					}

					if (!await GoodsSQL.EditCategory(category.Kategorie))
					{
						return await ReturnBad();
					}
				}
				else
				{
					return await ReturnBad();
				}

				async Task<IActionResult> ReturnBad()
				{
					category.NadrazenaKategorie = await GoodsSQL.GetAllCategoriesIdNameAcronym();
					return View("EditCategory", category);
				}
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané kategorie
		/// </summary>
		/// <param name="index">ID odstraňované kategorie</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteCategory(int index)
		{
			if (Role == Roles.Admin)
			{
				string? output = await GoodsSQL.DeleteCategory(index);
				if(output != null)
				{
					ViewBag.ErrorInfo = output;
					List<Kategorie_NadrazenaKategorie> categories = await GoodsSQL.GetAllCategories();
					return View("ListCategories", categories);
				}
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}


		/// <summary>
		/// Výpis veškerého zboží
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListGoods()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Skladnik || Role == Roles.Logistik)
			{
				List<Zbozi_Um_Kat_Dod_Soubory> zboziUmisteniKategorieDodavatel = await GoodsSQL.GetAllGoodsWithLocationCategorySupplier();
				return View(zboziUmisteniKategorieDodavatel);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všeho zboží
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchGoods(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				ViewBag.Search = search;
				List<Zbozi_Um_Kat_Dod_Soubory> zbozi = await GoodsSQL.GetAllGoodsWithLocationCategorySupplier();
				if (search != null)
				{
					search = search.ToLower();
					zbozi = zbozi.Where(lmb => (lmb.Zbozi?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Zbozi?.JednotkovaCena != null && lmb.Zbozi.JednotkovaCena.ToString().ToLower().Contains(search)) || (lmb.Zbozi?.PocetNaSklade != null && lmb.Zbozi.PocetNaSklade.ToString().ToLower().Contains(search)) || (lmb.Zbozi?.CarovyKod ?? string.Empty).ToLower().Contains(search) || (lmb.Zbozi?.Poznamka ?? string.Empty).ToLower().Contains(search) || (lmb.Kategorie?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Kategorie?.Zkratka ?? string.Empty).ToLower().Contains(search) || (lmb.Kategorie?.Popis ?? string.Empty).ToLower().Contains(search) || (lmb.Umisteni?.Mistnost ?? string.Empty).ToLower().Contains(search) || (lmb.Umisteni?.Rada ?? string.Empty).ToLower().Contains(search) || (lmb.Umisteni?.Regal ?? string.Empty).ToLower().Contains(search) || (lmb.Umisteni?.Pozice ?? string.Empty).ToLower().Contains(search) || (lmb.Umisteni?.Datum != null && lmb.Umisteni.Datum.ToString().ToLower().Contains(search)) || (lmb.Dodavatele?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Jmeno ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Prijmeni ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Telefon ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Email ?? string.Empty).ToLower().Contains(search)).ToList();
				}
				return View(nameof(ListGoods), zbozi);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání nového zboží
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddGoods()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Zbozi_Um_Kat_DodList goods = new();
				goods.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
				goods.Umisteni = await GoodsSQL.GetAllLocationsPositions();
				goods.Dodavatele = await SupplierSQL.GetAllSuppliersName();
				return View(goods);
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Metoda převede obrázek do pole bytů a přidá dodačné informace
		/// </summary>
		/// <param name="file">Vstupní obrázek</param>
		/// <returns>Model Souboru s upravenými daty</returns>
		private async Task<Soubory> CreateImage(IFormFile file)
		{
			Soubory newFile = new();
			if (file != null && file.Length > 0)
			{
				newFile.IdZamestnance = await EmployeeSQL.GetEmployeeIdByEmail(Email);
				newFile.Nazev = file.FileName;
				string[] arr = file.ContentType.Split("/");
				newFile.TypSouboru = arr[0];
				newFile.PriponaSouboru = arr[1];
				using (var memoryStream = new MemoryStream())
				{
					file.CopyTo(memoryStream);
					newFile.Data = memoryStream.ToArray();
				}
			}
			return newFile;
		}

		/// <summary>
		/// Příjem dat z formuláře na přidání nového zboží
		/// </summary>
		/// <param name="newGoods">Model s daty nového zboží</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddGoods(Zbozi_Um_Kat_DodList newGoods, IFormFile soubor)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				ModelState.Remove("soubor");
				if (ModelState.IsValid)
				{
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (await GoodsSQL.CheckExistsBarcode(newGoods.Zbozi.CarovyKod) == true)
					{
						ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
						return await ReturnBad();
					}

					string? err = await GoodsSQL.AddGoods(newGoods.Zbozi, await CreateImage(soubor));

					if (err == null)
					{
						return RedirectToAction(nameof(ListGoods), nameof(Goods));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return await ReturnBad();
					}
				}
				return await ReturnBad();
			}

			async Task<IActionResult> ReturnBad()
			{
				newGoods.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
				newGoods.Umisteni = await GoodsSQL.GetAllLocationsPositions();
				newGoods.Dodavatele = await SupplierSQL.GetAllSuppliersName();
				return View(newGoods);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybraného zboží
		/// </summary>
		/// <param name="index">ID upravovaného zboží</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditGoodsGet(int index)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Zbozi_Um_Kat_DodList goods = new();
				goods.Zbozi = await GoodsSQL.GetGoodsById(index);
				goods.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
				goods.Umisteni = await GoodsSQL.GetAllLocationsPositions();
				goods.Dodavatele = await SupplierSQL.GetAllSuppliersName();
				return View("EditGoods", goods);
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Příjem upravených dat vybraného zboží
		/// </summary>
		/// <param name="editZbozi">Model s upravenými daty zboží</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditGoodsPost(Zbozi_Um_Kat_DodList editZbozi, IFormFile soubor, bool checkBoxDelete)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				ModelState.Remove("soubor");
				ModelState.Remove("checkBoxDelete");
				if (ModelState.IsValid)
				{
					string? barcode = await GoodsSQL.GetBarcodeByIdGoods(editZbozi.Zbozi.IdZbozi);
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (!barcode.Equals(editZbozi.Zbozi.CarovyKod))
					{
						if (await GoodsSQL.CheckExistsBarcode(editZbozi.Zbozi.CarovyKod))
						{
							ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
							return await ReturnBad();
						}
					}

					if (checkBoxDelete == true)
					{
						editZbozi.Zbozi.IdSouboru = -1;
					}

					string? err = await GoodsSQL.EditGoods(editZbozi.Zbozi, await CreateImage(soubor));

					if (err == null)
					{
						return RedirectToAction(nameof(ListGoods), nameof(Goods));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return await ReturnBad();
					}
				}
				return await ReturnBad();

				async Task<IActionResult> ReturnBad()
				{
					editZbozi.Kategorie = await GoodsSQL.GetAllCategoriesNameAcronym();
					editZbozi.Umisteni = await GoodsSQL.GetAllLocationsPositions();
					editZbozi.Dodavatele = await SupplierSQL.GetAllSuppliersName();
					return View("EditGoods", editZbozi);
				}
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zboží
		/// </summary>
		/// <param name="index">ID odstraňovaného zboží</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteGoods(int index)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_ZBOZI", index);
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Výpis všech umístění
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListLocations()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				Zbozi_Pohyby_Umisteni goods = new();
				goods.Zbozi = await GoodsSQL.GetAllGoodsIdName();
				goods.Movements = await GoodsSQL.GetAllLocationsMovements();
				goods.Umisteni = await GoodsSQL.GetAllLocations();
				return View(goods);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech umístění
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchLocations(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				ViewBag.Search = search;
				Zbozi_Pohyby_Umisteni goods = new();
				goods.Zbozi = await GoodsSQL.GetAllGoodsIdName();
				goods.Movements = await GoodsSQL.GetAllLocationsMovements();
				goods.Umisteni = await GoodsSQL.GetAllLocations();
				if (search != null)
				{
					search = search.ToLower();
					goods.Umisteni = goods.Umisteni.Where(lmb => (lmb.Mistnost ?? string.Empty).ToLower().Contains(search) || (lmb.Rada ?? string.Empty).ToLower().Contains(search) || (lmb.Regal ?? string.Empty).ToLower().Contains(search) || (lmb.Pozice ?? string.Empty).ToLower().Contains(search) || (lmb.Datum != null && lmb.Datum.ToString().ToLower().Contains(search)) || (lmb.Poznamka ?? string.Empty).ToLower().Contains(search)).ToList();
					if (goods.Umisteni.Count == 0)
					{
						goods.Umisteni = await GoodsSQL.GetAllLocations();
						goods.Zbozi = goods.Zbozi.Where(lmb => (lmb.Nazev ?? string.Empty).ToLower().Contains(search)).ToList();
						var idUmisteniPohybyList = goods.Zbozi.Select(lmb => lmb.IdUmisteni).ToList();
						goods.Umisteni = goods.Umisteni.Where(lmb => idUmisteniPohybyList.Contains(lmb.IdUmisteni)).ToList();
						goods.Movements = goods.Movements.Where(lmb => goods.Zbozi.Any(x => x.IdZbozi == lmb.IdZbozi)).ToList();
					}
				}
				return View(nameof(ListLocations), goods);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání nového umístění
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult AddLocation()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				return View();
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		/// <summary>
		/// Příjem dat z formuláře na přidání umístění
		/// </summary>
		/// <param name="newLocation">Model s daty nového umístění</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddLocation(Umisteni newLocation)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				if (ModelState.IsValid)
				{
					if (await GoodsSQL.AddLocation(newLocation))
					{
						return RedirectToAction(nameof(ListLocations), nameof(Goods));
					}
				}
				return View(newLocation);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybraného umístění
		/// </summary>
		/// <param name="index">ID upravovaného umístění</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditLocationGet(int index)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				Umisteni umisteni = await GoodsSQL.GetLocationById(index);
				return View("EditLocation", umisteni);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		/// <summary>
		// Příjem upravených dat vybraného umístění
		/// </summary>
		/// <param name="umisteni"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditLocationPost(Umisteni umisteni)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				if (ModelState.IsValid)
				{
					if (await GoodsSQL.EditLocation(umisteni))
					{
						return RedirectToAction(nameof(ListLocations), nameof(Goods));
					}
				}
				return View("EditLocation", umisteni);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		/// <summary>
		/// Odstranění vybraného umístění
		/// </summary>
		/// <param name="index">ID odstraňovaného umístění</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteLocation(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_UMISTENI", index);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}
	}
}