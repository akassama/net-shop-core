﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModestLiving.Models;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBConnection _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(DBConnection context)
        {
           // _logger = logger;

            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //Get the top 3 from collections (catefories)
            ViewBag.CollectionsData = _context.Categories.OrderByDescending(s => s.ID).Take(3);

            var data = _context.Products.ToListAsync();

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
