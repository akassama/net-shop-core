using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly DBConnection _context;

        public CollectionsController(DBConnection context)
        {

            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _context.Categories.OrderBy(s=> s.ID).ToListAsync());
        }
        public async Task<IActionResult> Category(string id)
        {
            try
            {
                //Check if string pass in not empty
                if (!String.IsNullOrEmpty(id))
                {
                    //get category id from category name
                    int category_id = _context.Categories.Where(s => s.CategoryName == id).FirstOrDefault().ID;

                    //return all products with the category id
                    return View(await _context.Products.Where(s => s.CategoryID == category_id).OrderBy(s => s.ID).ToListAsync());
                }
            }
            catch(Exception ex)
            {
                //TODO Log error
                Console.WriteLine(ex);
            }
            //return home if bad parameter passed
            return RedirectToAction("Index", "Home");
        }

    }
}