using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;
        [BindProperty]
        public ProductsViewModel ProductVM { get; set; }
        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductVM = new ProductsViewModel
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(ProductVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductVM);
            }
            _db.Products.Add(ProductVM.Products);
            await _db.SaveChangesAsync();

            //image saving
            string webrootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productFromDb = _db.Products.Find(ProductVM.Products.Id);
            if (files.Count != 0)
            {
                //image uploaded
                var uploads = Path.Combine(webrootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                productFromDb.Image = @"/" + SD.ImageFolder + @"/" + ProductVM.Products.Id + extension;

            }

            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webrootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webrootPath + @"\" + SD.ImageFolder + @"\" + ProductVM.Products.Id + ".png");
                productFromDb.Image = @"/" + SD.ImageFolder + @"/" + ProductVM.Products.Id + ".png";

            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



        //get edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductVM);

        }

        //Post :Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webrootpath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var productfromdb = _db.Products.Where(m => m.Id == ProductVM.Products.Id).FirstOrDefault();
                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new file
                    var uploads = Path.Combine(webrootpath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productfromdb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProductVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductVM.Products.Id + extension_old));

                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    ProductVM.Products.Image = @"/" + SD.ImageFolder + @"/" + ProductVM.Products.Id + extension_new;
                }

                if (ProductVM.Products.Image != null)
                {
                    productfromdb.Image = ProductVM.Products.Image;
                }

                productfromdb.Name = ProductVM.Products.Name;
                productfromdb.Price = ProductVM.Products.Price;
                productfromdb.Available = ProductVM.Products.Available;
                productfromdb.ProductTypeId = ProductVM.Products.ProductTypeId;
                productfromdb.SpedialTagID = ProductVM.Products.SpedialTagID;
                productfromdb.ShadeColor = ProductVM.Products.ShadeColor;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ProductVM);
        }

        //Details get
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductVM);

        }
        //Get Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductVM);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            string webrootpath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            else
            {
                var uploads = Path.Combine(webrootpath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);

                if(System.IO.File.Exists(Path.Combine(uploads,products.Id+extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }
                _db.Remove(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}