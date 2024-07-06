﻿using GraniteHouse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraniteHouse.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userfromdb = await _db.ApplictionUser.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();

            return View(userfromdb);
        }
    }
}