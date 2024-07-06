using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public SHoppingCartViewModel ShoppingViewModel { get; set; }
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingViewModel = new SHoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            }; 
        }
        public async Task<IActionResult> Index()
        {
            List<int> listfromsesion = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listfromsesion.Count >0)
            {
                foreach(int item in listfromsesion)
                {
                    Products prod = _db.Products.Include(p => p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == item).FirstOrDefault();
                    ShoppingViewModel.Products.Add(prod);
                }
                return View(ShoppingViewModel);
            }
            else
            return View(ShoppingViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> ListCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            Appointments appointments = ShoppingViewModel.Appointments;

            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentid = appointments.Id;

            foreach(var productid in ListCartItems)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment
                {
                   AppointmentId = appointmentid,
                   ProductId = productid,
                };
                _db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);
            }
            _db.SaveChanges();
            ListCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", ListCartItems);

            return RedirectToAction("AppointmentConfirmation","ShoppingCart", new {Id = appointmentid});
        }

        public IActionResult Remove(int id)
        {
            List<int> ListCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (ListCartItems.Count>0)
            {
                if (ListCartItems.Contains(id))
                {
                   ListCartItems.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", ListCartItems);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingViewModel.Appointments = _db.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsSelectedForAppointment> objproList = _db.ProductsSelectedForAppointment.Where(p => p.AppointmentId == id).ToList();

            foreach(ProductsSelectedForAppointment prodobj in objproList)
            {
                ShoppingViewModel.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodobj.ProductId).FirstOrDefault());
            }
            return View(ShoppingViewModel);
        }
    }
}