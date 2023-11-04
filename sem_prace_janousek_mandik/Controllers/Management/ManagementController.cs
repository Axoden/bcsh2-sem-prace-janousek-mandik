using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public class ManagementController : BaseController
	{
		// Výpis všech pozic
		public IActionResult ListPositions()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin"))
			{
				List<Pozice> pozice = ManagementSQL.GetAllPositions();
				ViewBag.ListOfPositions = pozice;

				return View();
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nové pozice
		[HttpGet]
		public IActionResult AddPosition()
		{
			// Dostupné pouze pro administrátora
			if (Role.Equals("Admin"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		// Příjem dat z formuláře na přidání pozice
		[HttpPost]
		public IActionResult AddPosition(Pozice novaPozice)
		{
			// Dostupné pouze pro administrátora
			if (Role.Equals("Admin"))
			{
				if (ModelState.IsValid == true)
				{
					bool uspesnaRegistrace = ManagementSQL.RegisterPosition(novaPozice);

					if (uspesnaRegistrace == true)
					{
						// Úspěšná registrace, přesměrování na výpis pozic
						return RedirectToAction(nameof(ListPositions), nameof(Management));
					}
				}
				return View(novaPozice);
			}

			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		// Načtení formuláře na úpravu vybrané pozice
		[HttpGet]
		public IActionResult EditPosition(int index)
		{
			// Kontrola oprávnění na načtení parametru pozice
			if (Role.Equals("Admin"))
			{
				Pozice pozice = ManagementSQL.GetPositionById(index);
				return View(pozice);
			}

			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		// Příjem upravených dat vybrané pozice
		[HttpPost]
		public IActionResult EditPosition(Pozice pozice, int idPozice)
		{
			if (Role.Equals("Admin"))
			{
				pozice.IdPozice = idPozice;
				ManagementSQL.EditPosition(pozice);
			}

			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}

		// Formální metoda pro odstranění vybrané pozice
		[HttpGet]
		public IActionResult DeletePosition(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_POZICI", index);
			}

			return RedirectToAction(nameof(ListPositions), nameof(Management));
		}
	}
}