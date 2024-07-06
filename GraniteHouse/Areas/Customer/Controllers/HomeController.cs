using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using GraniteHouse.Data;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Extensions;

namespace GraniteHouse.Controllers

{

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(productList);
        }

        //Get
        public async Task<IActionResult>Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == id).FirstOrDefaultAsync();

            return View(product);
        }
        //Post
        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            List<int> lstSHoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(lstSHoppingCart ==null)
            {
                lstSHoppingCart = new List<int>();
            }
            lstSHoppingCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", lstSHoppingCart);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult Remove(int id)
        {
            List<int> lstSHoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstSHoppingCart.Count>0)
            {
                if(lstSHoppingCart.Contains(id))
                {
                    lstSHoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", lstSHoppingCart);
            return RedirectToAction(nameof(Index));
                
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
