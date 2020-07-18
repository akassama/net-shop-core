using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBConnection _context;


        private readonly SystemConfiguration _systemConfiguration;

        public HomeController(DBConnection context, IOptions<SystemConfiguration> systemConfiguration)
        {
            _context = context;
            _systemConfiguration = systemConfiguration.Value;
        }

        public async Task<IActionResult> Index()
        {
            //Get the top 3 from collections (catefories)
            ViewBag.CollectionsData = _context.Categories.OrderByDescending(s => s.ID).Take(3);

            //Get recent products 
            int totalProducts = _systemConfiguration.totalHomeProducts;
            var data = _context.Products.OrderByDescending(s => s.ID).Take(totalProducts).ToListAsync(); 

            return View(await data);
        }

        //public async Task<IActionResult> Index()
        //{
        //    //get last 9 products
        //    var data = _context.Products.Where(s=> s.ApproveStatus == 1).OrderByDescending(s=> s.ID).ToListAsync();

        //    //return View(data);
        //     return View(await _context.Products.ToListAsync().ConfigureAwait(false));
        //}


        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
