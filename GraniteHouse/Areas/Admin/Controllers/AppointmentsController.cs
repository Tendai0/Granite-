﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        public readonly ApplicationDbContext _db;
        private int PageSize = 5;
        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int productPage=1,string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVm = new AppointmentViewModel()
            {

                Appointments = new List<Models.Appointments>()

            };

            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if(searchName!=null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }
           
            appointmentVm.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();

            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVm.Appointments = appointmentVm.Appointments.Where(u => u.SalesPersonId == claim.Value).ToList();
            }

            if(searchName != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (searchEmail!= null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {


                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate); 
                    appointmentVm.Appointments = appointmentVm.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();

                }

                catch (Exception ex)
                {

                }
                    
            }

            var count = appointmentVm.Appointments.Count();
            appointmentVm.Appointments = appointmentVm.Appointments.OrderBy(a => a.AppointmentDate)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            appointmentVm.PagingInfo = new PagingInfo
            {
                CurrenPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()

            };

            return View(appointmentVm);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var productsList = (IEnumerable<Products>)(from p in _db.Products
                                                       join a in _db.ProductsSelectedForAppointment
                                                       on p.Id equals a.ProductId
                                                       where a.AppointmentId == id

                                                       select p).Include("ProductTypes");

            AppointmentDetailsViewModel objappointVM = new AppointmentDetailsViewModel()
            {
                Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplictionUser.ToList(),
                Products = productsList.ToList()
            };
            return View(objappointVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objappoint)
        {
            if (ModelState.IsValid)
            {
                var appointmentfromdb = _db.Appointments.Where(a => a.Id == objappoint.Appointments.Id).FirstOrDefault();

                appointmentfromdb.CustomerName = objappoint.Appointments.CustomerName;
                appointmentfromdb.CustomerEmail = objappoint.Appointments.CustomerEmail;
                appointmentfromdb.CustomerPhoneNumber = objappoint.Appointments.CustomerPhoneNumber;
                appointmentfromdb.AppointmentDate = objappoint.Appointments.AppointmentDate;
                appointmentfromdb.isConfirmed = objappoint.Appointments.isConfirmed;

                if(User.IsInRole(SD.SuperAdminEndUser))
                {
                    appointmentfromdb.SalesPersonId = objappoint.Appointments.SalesPersonId;
                }
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(objappoint);
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var productsList = (IEnumerable<Products>)(from p in _db.Products
                                                       join a in _db.ProductsSelectedForAppointment
                                                       on p.Id equals a.ProductId
                                                       where a.AppointmentId == id

                                                       select p).Include("ProductTypes");

            AppointmentDetailsViewModel objappointVM = new AppointmentDetailsViewModel()
            {
                Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplictionUser.ToList(),
                Products = productsList.ToList()
            };
            return View(objappointVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var productsList = (IEnumerable<Products>)(from p in _db.Products
                                                       join a in _db.ProductsSelectedForAppointment
                                                       on p.Id equals a.ProductId
                                                       where a.AppointmentId == id

                                                       select p).Include("ProductTypes");

            AppointmentDetailsViewModel objappointVM = new AppointmentDetailsViewModel()
            {
                Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplictionUser.ToList(),
                Products = productsList.ToList()
            };
            return View(objappointVM);
        }
        //post delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
              var appointment = await _db.Appointments.FindAsync(id);
             _db.Appointments.Remove(appointment);
              await  _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}