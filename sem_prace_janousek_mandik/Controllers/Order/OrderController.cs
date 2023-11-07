using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Goods;
using sem_prace_janousek_mandik.Controllers.Payment;
using sem_prace_janousek_mandik.Controllers.Supplier;
using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Order;
using sem_prace_janousek_mandik.Models.Payment;
using System;
using System.Text.Json;

namespace sem_prace_janousek_mandik.Controllers.Order
{
    public class OrderController : BaseController
    {
        // Výpis všech objednávek včetně objednaného zboží
        public IActionResult ListOrders()
        {
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
            {
                List<Objednavky_Zamestnanci_Zakaznici> objednavky = OrderSQL.GetAllOrders();
                List<ZboziObjednavek_Zbozi> zboziObjednavek = OrderSQL.GetAllGoodsOrders();
                ViewBag.ListOfGoodsOrders = zboziObjednavek;

                return View(objednavky);
            }

            if (Role.Equals("Zakaznik"))
            {
                List<Objednavky_Zamestnanci> objednavky = OrderSQL.GetAllCustomerOrders(Email);
                ViewBag.ListOfOrders = objednavky;
                List<ZboziObjednavek_Zbozi> zboziObjednavek = OrderSQL.GetAllGoodsOrdersCustomer(Email);
                ViewBag.ListOfGoodsOrders = zboziObjednavek;
                return View();
            }

            // Přesměrování, pokud uživatel nemá povolen přístup
            return RedirectToAction("Index", "Home");
        }

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

        public IActionResult AddOrder()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditOrder(int index)
        {
            if (Role.Equals("Admin"))
            {
                Objednavky objednavka = OrderSQL.GetOrderById(index);
                List<Faktury> invoices = PaymentSQL.GetAllInvoices();
                List<Zamestnanci> employees = EmployeeSQL.GetAllEmployees();
                List<Zakaznici> customers = CustomerSQL.GetAllCustomers();
                ViewBag.ListOfInvoices = invoices;
                ViewBag.ListOfEmployees = employees;
                ViewBag.ListOfCustomers = customers;

                return View(objednavka);
            }

            return RedirectToAction(nameof(ListOrders), nameof(Order));
        }

        [HttpGet]
        public IActionResult AddGoodsToOrder(int idObjednavky)
        {
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                // Kontrola, zda je objednávka otevřena
                if (OrderSQL.ClosedOrder(idObjednavky) == false)
                {
                    ViewBag.IdObjednavky = idObjednavky;
                    ViewBag.ListOfGoods = OrderSQL.GetAllGoods();
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddGoodsToOrder(ZboziObjednavek_Zbozi addZboziObjednavek, int idObjednavky)
        {
            
            if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
            {
                // Kontrola, zda je objednávka otevřena
                if (OrderSQL.ClosedOrder(idObjednavky) == false)
                {
                    bool uspesnePridani = OrderSQL.AddGoodsToOrder(addZboziObjednavek);

                    if (uspesnePridani == true)
                    {
                        // Úspěšná registrace, přesměrování na výpis kategorií
                        return RedirectToAction(nameof(ListOrders), nameof(Order));
                    }
                }
            }
            return View();
        }
    }
}
