using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_shop_core.Models;
using ModestLiving.Models;

namespace ModestLiving.Controllers
{
    public class NewArrivalsController : Controller
    {
        private readonly DBConnection _context;


        public NewArrivalsController(DBConnection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //Get the last 6 products from db
            var data = _context.Products.OrderByDescending(s => s.ID).Take(6).ToListAsync(); 

            return View(await data);
        }
    }
}