using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;    
        }
        public IActionResult Index()
        {
            return View(_db.ApplictionUser.ToList());
        }
        //Get
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            var userfromdb = await _db.ApplictionUser.FindAsync(id);
            if(userfromdb == null)
            {
                return NotFound();
            }

            return View(userfromdb);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplictionUser applictionUser)
        {
            if(id!=applictionUser.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                ApplictionUser userformbd = _db.ApplictionUser.Where(a => a.Id == id).FirstOrDefault();
                userformbd.Name = applictionUser.Name;
                userformbd.PhoneNumber = applictionUser.PhoneNumber;
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(applictionUser);
        }

        //Get
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            var userfromdb = await _db.ApplictionUser.FindAsync(id);
            if (userfromdb == null)
            {
                return NotFound();
            }

            return View(userfromdb);
        }

        //Post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string id)
        {
           
                ApplictionUser userformbd = _db.ApplictionUser.Where(a => a.Id == id).FirstOrDefault();
                userformbd.LockoutEnd = DateTime.Now.AddYears(1000);
                _db.SaveChanges();

                 return RedirectToAction(nameof(Index));
           
        }
    }
}