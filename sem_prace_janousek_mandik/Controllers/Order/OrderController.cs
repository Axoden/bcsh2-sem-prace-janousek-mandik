using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Payment;
using sem_prace_janousek_mandik.Models.Order;

namespace sem_prace_janousek_mandik.Controllers.Order
{
    public class OrderController : BaseController
    {
        // Výpis všech objednávek včetně objednaného zboží
        public IActionResult ListOrders()
        {
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
            {
                List<Objednavky_Zamestnanci_Zakaznici_Faktury> objednavky = OrderSQL.GetAllOrders();
                ViewBag.ListOfGoodsOrders = OrderSQL.GetAllGoodsOrders();
				ViewBag.ListOfPayments = PaymentSQL.GetAllPayments();

				return View(objednavky);
            }

            if (Role.Equals("Zakaznik"))
            {
                ViewBag.ListOfOrders = OrderSQL.GetAllCustomerOrders(Email);
                ViewBag.ListOfGoodsOrders = OrderSQL.GetAllGoodsOrdersCustomer(Email);
				ViewBag.ListOfPayments = PaymentSQL.GetAllPayments();

				return View();
            }

            // Přesměrování, pokud uživatel nemá povolen přístup
            return RedirectToAction("Index", "Home");
        }

		// Metoda zpracování dat nové objednávky
		[HttpGet]
        public IActionResult EditGoodsOrder(int index)
        {
            // Kontrola oprávnění na načtení parametrů zboží
            if (Role.Equals("Admin"))
            {
                ZboziObjednavek_Zbozi zboziObjednavek = OrderSQL.GetGoodsOrderById(index);
                return View(zboziObjednavek);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro zpracování upravených dat konkrétního zboží z objednávky
		[HttpPost]
        public IActionResult EditGoodsOrder(ZboziObjednavek_Zbozi zboziObjednavky, int index)
        {
            if (Role.Equals("Admin"))
            {
                zboziObjednavky.ZboziObjednavek.IdZboziObjednavky = index;
                OrderSQL.EditGoodsOrder(zboziObjednavky);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro načtení formuláře na vytvoření objednávky
		[HttpGet]
        public IActionResult AddOrder()
        {
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                ViewBag.ListOfCustomers = CustomerSQL.GetAllCustomersNameSurname();
                return View();
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro zpracování dat nové objednávky
		[HttpPost]
        public IActionResult AddOrder(Objednavy_Faktury newOrder)
        {
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                //if (ModelState.IsValid == true)
                //{
                newOrder.Objednavky.IdZamestnance = EmployeeSQL.GetEmployeeIdByEmail(Email);
                bool uspesnePridani = OrderSQL.AddOrder(newOrder);

                if (uspesnePridani == true)
                {
                    return RedirectToAction(nameof(ListOrders), nameof(Order));
                }
                //}
                return View(newOrder);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro načtení formuláře na úpravu objednávky
		[HttpGet]
        public IActionResult EditOrder(int index)
        {
            if (Role.Equals("Admin"))
            {
                Objednavky objednavka = OrderSQL.GetOrderById(index);
                ViewBag.ListOfInvoices = PaymentSQL.GetAllInvoices();
                ViewBag.ListOfEmployees = EmployeeSQL.GetAllEmployeesNameSurname();
                ViewBag.ListOfCustomers = CustomerSQL.GetAllCustomersNameSurname();

                return View(objednavka);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro načtení formuláře na přidání zboží do objednávky
		[HttpGet]
        public IActionResult AddGoodsToOrder(int idObjednavky)
        {
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                // Kontrola, zda je objednávka otevřena
                if (OrderSQL.IsClosedOrder(idObjednavky) == false)
                {
                    ViewBag.IdObjednavky = idObjednavky;
                    ViewBag.ListOfGoods = OrderSQL.GetAllGoods();
                }
            }
            return View();
        }

		// Metoda pro zpracování dat přidání zboží do objednávky
		[HttpPost]
        public IActionResult AddGoodsToOrder(ZboziObjednavek_Zbozi addZboziObjednavek, int idObjednavky)
        {

            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                // Kontrola, zda je objednávka otevřena
                if (OrderSQL.IsClosedOrder(idObjednavky) == false)
                {
                    // TODO: Validace vstupu
                    float jednotkovaCena = OrderSQL.GetPriceForGoods(addZboziObjednavek.ZboziObjednavek.IdZbozi);
                    addZboziObjednavek.ZboziObjednavek.JednotkovaCena = jednotkovaCena;
                    addZboziObjednavek.ZboziObjednavek.IdObjednavky = idObjednavky;
                    bool uspesnePridani = OrderSQL.AddGoodsToOrder(addZboziObjednavek);

                    if (uspesnePridani == true)
                    {
                        return RedirectToAction(nameof(ListOrders), nameof(Order));
                    }
                }
            }
            return View();
        }

		// Metoda pro uzavření objednávky
		public IActionResult CloseOrder(int idObjednavky)
        {
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                OrderSQL.CloseOrder(idObjednavky);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

		// Metoda pro odstranění vybrané objednávky
		[HttpGet]
        public IActionResult DeleteOrder(int index)
        {
            if (Role.Equals("Admin"))
            {
                SharedSQL.CallDeleteProcedure("P_SMAZAT_OBJEDNAVKU", index);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

        // Metoda pro odstranění vybraného zboží z objednávky
        [HttpGet]
        public IActionResult DeleteGoodsOrder(int index)
        {
            if (Role.Equals("Admin"))
            {
                SharedSQL.CallDeleteProcedure("P_SMAZAT_ZBOZI_OBJEDNAVEK", index);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }
    }
}
