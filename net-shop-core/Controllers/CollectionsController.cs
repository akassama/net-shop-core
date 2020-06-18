using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly DBConnection _context;
        private readonly SystemConfiguration _systemConfiguration;

        public CollectionsController(DBConnection context, IOptions<SystemConfiguration> systemConfiguration)
        {
            _context = context;
            _systemConfiguration = systemConfiguration.Value;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.OrderBy(s=> s.ID).ToListAsync());
        }
        public IActionResult Category(string id, int page = 0)
        {
            try
            {
                //Check if string pass in not empty
                if (!string.IsNullOrEmpty(id))
                {
                    //get category id from category name
                    int category_id = _context.Categories.Where(s => s.CategoryName == id).FirstOrDefault().ID;

                    //Get products from db
                    var dataSource = _context.Products.Where(s => s.CategoryID == category_id && s.ApproveStatus == 1).OrderByDescending(s => s.ID);

                    //Page size from config file
                    int PageSize = _systemConfiguration.categoryPageSize;

                    int count = dataSource.Count();

                    var data = dataSource.Skip(page * PageSize).Take(PageSize).ToList();

                    ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                    ViewBag.Page = page;

                    //return all products with the category id
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                //TODO Log error
                Console.WriteLine(ex);
            }
            //return home if bad parameter passed
            return RedirectToAction("Index", "Home");
        }

    }
}