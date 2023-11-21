using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Controllers.Supplier;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Employee;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
    public class GoodsController : BaseController
	{
		/// <summary>
		/// Výpis všech kategorií
		/// </summary>
		/// <returns></returns>
		public IActionResult ListCategories()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				List<Kategorie_NadrazenaKategorie> categories = GoodsSQL.GetAllCategories();
				return View(categories);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech kategorií
		/// </summary>
		/// <param name="search">Vstup pro vyhledávání</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchCategories(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				// Předání vyhledávaného výrazu
				ViewBag.Search = search;
				List<Kategorie_NadrazenaKategorie> categories = GoodsSQL.GetAllCategories();
				if (search != null)
				{
					categories = categories.Where(lmb => lmb.Kategorie.Nazev.ToLower().Contains(search.ToLower()) || lmb.Kategorie.Zkratka.ToLower().Contains(search.ToLower()) ||
					lmb.Kategorie.Popis.ToLower().Contains(search.ToLower()) || lmb.NadrazenaKategorie.Nazev.ToLower().Contains(search.ToLower())).ToList();
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
		public IActionResult AddCategory()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Kategorie_NadrazenaKategorie_List category = new();
				category.NadrazenaKategorie = GoodsSQL.GetAllCategoriesIdNameAcronym();
				return View(category);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		/// <summary>
		// Příjem dat z formuláře na přidání kategorie
		/// </summary>
		/// <param name="newCategory">Model s daty nové kategorie</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddCategory(Kategorie_NadrazenaKategorie_List newCategory)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				newCategory.NadrazenaKategorie = GoodsSQL.GetAllCategoriesIdNameAcronym();
				if (ModelState.IsValid == true)
				{
					newCategory.Kategorie.Zkratka = newCategory.Kategorie.Zkratka.ToUpper();
					// Kontrola zda již neexistuje kategorie s touto zkratkou
					if (GoodsSQL.CheckExistsAcronym(newCategory.Kategorie.Zkratka) == true)
					{
						ViewBag.ErrorInfo = "Tato zkratka je již používána!";
						return View(newCategory);
					}

					if (GoodsSQL.AddCategory(newCategory.Kategorie))
					{
						// Úspěšné přidání, přesměrování na výpis kategorií
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
		public IActionResult EditCategoryGet(int index)
		{
			// Kontrola oprávnění na načtení parametrů kategorie
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Kategorie_NadrazenaKategorie_List category = new();
				category.Kategorie = GoodsSQL.GetCategoryById(index);
				category.NadrazenaKategorie = GoodsSQL.GetAllCategoriesIdNameAcronym();
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
		public IActionResult EditCategoryPost(Kategorie_NadrazenaKategorie_List category)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid)
				{
					category.Kategorie.Zkratka = category.Kategorie.Zkratka.ToUpper();
					string acronym = GoodsSQL.GetAcronymByIdCategory(category.Kategorie.IdKategorie);
					// Kontrola zda již neexistuje kategorie s touto zkratkou
					if (!acronym.Equals(category.Kategorie.Zkratka))
					{
						if (GoodsSQL.CheckExistsAcronym(category.Kategorie.Zkratka) == true)
						{
							ViewBag.ErrorInfo = "Tato zkratka je již používána!";
							return ReturnBad();
						}
					}

					if (!GoodsSQL.EditCategory(category.Kategorie))
					{
						return ReturnBad();
					}
				}
				else
				{
					return ReturnBad();
				}

				IActionResult ReturnBad()
				{
					category.NadrazenaKategorie = GoodsSQL.GetAllCategoriesIdNameAcronym();
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
		public IActionResult DeleteCategory(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_KATEGORII", index);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}


		/// <summary>
		/// Výpis veškerého zboží
		/// </summary>
		/// <returns></returns>
		public IActionResult ListGoods()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Zbozi_Um_Kat_Dod_Soubory> zboziUmisteniKategorieDodavatel = GoodsSQL.GetAllGoodsWithLocationCategorySupplier();
				return View(zboziUmisteniKategorieDodavatel);
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všeho zboží
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchGoods(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				ViewBag.Search = search;
				List<Zbozi_Um_Kat_Dod_Soubory> zbozi = GoodsSQL.GetAllGoodsWithLocationCategorySupplier();
				if (search != null)
				{
					zbozi = zbozi.Where(lmb => lmb.Zbozi.Nazev.ToLower().Contains(search.ToLower()) || lmb.Zbozi.JednotkovaCena.ToString().ToLower().Contains(search.ToLower()) || lmb.Zbozi.PocetNaSklade.ToString().ToLower().Contains(search.ToLower()) || lmb.Zbozi.CarovyKod.ToLower().Contains(search.ToLower()) || lmb.Zbozi.Poznamka.ToLower().Contains(search.ToLower()) || lmb.Kategorie.Nazev.ToLower().Contains(search.ToLower()) || lmb.Kategorie.Zkratka.ToLower().Contains(search.ToLower()) ||
					lmb.Kategorie.Popis.ToLower().Contains(search.ToLower()) || lmb.Umisteni.Mistnost.ToLower().Contains(search.ToLower()) || lmb.Umisteni.Rada.ToLower().Contains(search.ToLower()) || lmb.Umisteni.Regal.ToLower().Contains(search.ToLower()) || lmb.Umisteni.Pozice.ToLower().Contains(search.ToLower()) || lmb.Umisteni.Datum.ToString().ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Nazev.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Jmeno.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Prijmeni.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Telefon.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Email.ToLower().Contains(search.ToLower())).ToList();
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
		public IActionResult AddGoods()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Zbozi_Um_Kat_DodList goods = new();
				goods.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				goods.Umisteni = GoodsSQL.GetAllLocationsPositions();
				goods.Dodavatele = SupplierSQL.GetAllSuppliersName();
				return View(goods);
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Metoda převede obrázek do pole bytů a přidá dodačné informace
		/// </summary>
		/// <param name="file">Vstupní obrázek</param>
		/// <returns>Model Souboru s upravenými daty</returns>
		private Soubory CreateImage(IFormFile file)
		{
			Soubory newFile = new();
			if (file != null && file.Length > 0)
			{
				newFile.idZamestnance = EmployeeSQL.GetEmployeeIdByEmail(Email);
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
		public IActionResult AddGoods(Zbozi_Um_Kat_DodList newGoods, IFormFile soubor)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ModelState.Remove("soubor");
				if (ModelState.IsValid == true)
				{
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (GoodsSQL.CheckExistsBarcode(newGoods.Zbozi.CarovyKod) == true)
					{
						ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
						return ReturnBad();
					}

					if (GoodsSQL.RegisterGoods(newGoods.Zbozi, CreateImage(soubor)))
					{
						return RedirectToAction(nameof(ListGoods), nameof(Goods));
					}
				}
				return ReturnBad();
			}

			IActionResult ReturnBad()
			{
				newGoods.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				newGoods.Umisteni = GoodsSQL.GetAllLocationsPositions();
				newGoods.Dodavatele = SupplierSQL.GetAllSuppliersName();
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
		public IActionResult EditGoodsGet(int index)
		{
			// Kontrola oprávnění na načtení parametrů zboží
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Zbozi_Um_Kat_DodList goods = new();
				goods.Zbozi = GoodsSQL.GetGoodsById(index);
				goods.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
				goods.Umisteni = GoodsSQL.GetAllLocationsPositions();
				goods.Dodavatele = SupplierSQL.GetAllSuppliersName();
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
		public IActionResult EditGoodsPost(Zbozi_Um_Kat_DodList editZbozi, IFormFile soubor)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ModelState.Remove("soubor");
				if (ModelState.IsValid)
				{
					string barcode = GoodsSQL.GetBarcodeByIdGoods(editZbozi.Zbozi.IdZbozi);
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (!barcode.Equals(editZbozi.Zbozi.CarovyKod))
					{
						if (GoodsSQL.CheckExistsBarcode(editZbozi.Zbozi.CarovyKod) == true)
						{
							ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
							return ReturnBad();
						}
					}

					if (!GoodsSQL.EditGoods(editZbozi.Zbozi, CreateImage(soubor)))
					{
						return ReturnBad();
					}
				}

				IActionResult ReturnBad()
				{
					editZbozi.Kategorie = GoodsSQL.GetAllCategoriesNameAcronym();
					editZbozi.Umisteni = GoodsSQL.GetAllLocationsPositions();
					editZbozi.Dodavatele = SupplierSQL.GetAllSuppliersName();
					return View(editZbozi);
				}
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zboží
		/// </summary>
		/// <param name="index">ID odstraňovaného zboží</param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult DeleteGoods(int index)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZBOZI", index);
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		/// <summary>
		/// Výpis všech umístění
		/// </summary>
		/// <returns></returns>
		public IActionResult ListLocations()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				List<Umisteni> umisteni = GoodsSQL.GetAllLocations();
				return View(umisteni);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech umístění
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchLocations(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				ViewBag.Search = search;
				List<Umisteni> umisteni = GoodsSQL.GetAllLocations();
				if (search != null)
				{
					umisteni = umisteni.Where(lmb => lmb.Mistnost.ToLower().Contains(search.ToLower()) || lmb.Rada.ToLower().Contains(search.ToLower()) ||
					lmb.Regal.ToLower().Contains(search.ToLower()) || lmb.Pozice.ToLower().Contains(search.ToLower()) || lmb.Datum.ToString().ToLower().Contains(search.ToLower()) || lmb.Poznamka.ToLower().Contains(search.ToLower())).ToList();
				}
				return View(nameof(ListLocations), umisteni);
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
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
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
		public IActionResult AddLocation(Umisteni newLocation)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				if (ModelState.IsValid == true)
				{
					if (GoodsSQL.AddLocation(newLocation))
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
		public IActionResult EditLocationGet(int index)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				Umisteni umisteni = GoodsSQL.GetLocationById(index);
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
		public IActionResult EditLocationPost(Umisteni umisteni)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				if (!GoodsSQL.EditLocation(umisteni))
				{
					return View("EditLocation", umisteni);
				}
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		/// <summary>
		/// Odstranění vybraného umístění
		/// </summary>
		/// <param name="index">ID odstraňovaného umístění</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteLocation(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_UMISTENI", index);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}
	}
}