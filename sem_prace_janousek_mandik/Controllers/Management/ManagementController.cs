using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Goods;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;

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
                return View(pozice);
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

        // Metoda emuluje admina za vybraného zákazníka
        public IActionResult StartEmulationCustomer(string emailCustomer)
        {
            if (Role.Equals("Admin"))
            {
                HttpContext.Session.SetString("emulatedEmail", Email);
                HttpContext.Session.SetString("email", emailCustomer);
                HttpContext.Session.SetString("role", "Zakaznik");
            }
            return RedirectToAction("Index", "Home");
        }

        // Metoda emuluje admina za vybraného zaměstnance
        public IActionResult StartEmulationEmployee(int idEmployee)
        {
            if (Role.Equals("Admin"))
            {
                // Vytáhnutí emailu a role zaměstnance
                Zamestnanci_Pozice employee = EmployeeSQL.GetEmployeeRoleEmailById(idEmployee);
                HttpContext.Session.SetString("emulatedEmail", Email);
                HttpContext.Session.SetString("email", employee.Zamestnanci.Email);
                HttpContext.Session.SetString("role", employee.Pozice.Nazev);
            }
            return RedirectToAction("Index", "Home");
        }

        // Metoda ukončí emulaci a přepne zpět na původního admina
        public IActionResult EndEmulation()
        {
            if (!EmulatedEmail.Equals(""))
            {
                string adminEmail = EmulatedEmail;
                HttpContext.Session.SetString("emulatedEmail", "");
                HttpContext.Session.SetString("email", adminEmail);
                HttpContext.Session.SetString("role", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        // Metoda vypíše všechny databázové objekty
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
            return RedirectToAction("Index", "Home");
        }

        // Metoda vypíše změny dat v tabulkách
        public IActionResult ListLogs()
        {
            if (Role.Equals("Admin"))
            {
                List<LogTableInsUpdDel> logs = ManagementSQL.GetAllLogs();

                return View(logs);
            }
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("Index", "Home");
        }

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
            return RedirectToAction("Index", "Home");
        }
    }
}