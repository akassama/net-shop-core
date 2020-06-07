using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net_shop_core.Models;
using ModestLiving.Models;
using Wangkanai.Detection.Services;

namespace ModestLiving.Controllers
{
    public class ShopController : Controller
    {
        private readonly DBConnection _context;

        AppFunctions functions = new AppFunctions();

        private readonly IDetectionService _detectionService;

        private static IHttpContextAccessor _accessor;



        public ShopController(DBConnection context, IDetectionService detectionService, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _context = context;
            _detectionService = detectionService;
        }

        public async Task<IActionResult> Index()
        {
            
            //Get the top 3 from collections (catefories)
            ViewBag.CollectionsData = _context.Categories.OrderBy(s => s.ID).Take(3);

            //Get the last 15 products from db
            var data = _context.Products.OrderByDescending(s => s.ID).Take(15).ToListAsync();

            return View(await data);

        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                //Check if string pass in not empty
                if (!string.IsNullOrEmpty(id))
                {
                    //get product id from product unique name
                    int product_id = _context.Products.Where(s => s.UniqueProductName == id).FirstOrDefault().ID;

                    var productsModel = await _context.Products.FirstOrDefaultAsync(m => m.ID == product_id);

                    //check if empty result
                    if (productsModel == null)
                    {
                        //TODO setup Notfound page
                        return NotFound();
                    }

                    //Get all product images
                    ViewBag.ProductImages = _context.ProductImages.Where(s=> s.ProductID == product_id).OrderBy(s => s.ID);

                    //Get product video (if any)
                    ViewBag.ProductVideos = _context.ProductVideos.Where(s => s.ProductID == product_id).OrderBy(s => s.ID);

                    //Get all product colors
                    ViewBag.ProductColors = _context.ProductColors.Where(s => s.ProductID == product_id);

                    //Get all product sizes
                    ViewBag.ProductSizes = _context.ProductSize.Where(s => s.ProductID == product_id);



                    //log product view to ProductViews table
                    //Get current ViewedProducts session
                    var ViewedProducts = (HttpContext.Session.GetString("ViewedProducts") != null) ? HttpContext.Session.GetString("ViewedProducts") : "";

                    //if not viewed already
                    if (!functions.IsProductViewed(product_id, ViewedProducts))
                    {
                        //TODO get visitor id
                        var visitor_id = "Test-Visitor";
                        var visitor_ip = functions.FormatVisitorIP(_accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString());
                        functions.LogProductView(product_id, visitor_id, visitor_ip, _detectionService.Browser.Name.ToString(), _detectionService.Device.Type.ToString(), null);

                        //if not null set to session data else set to empty
                        HttpContext.Session.SetString("ViewedProducts", ViewedProducts + ",product_id_" + product_id.ToString());
                    }

                    //Return view with product model data
                    return View(productsModel);
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

        [HttpGet()]
        public async Task<IActionResult> Search([FromQuery(Name = "q")] string query)
        {

            //If search query passed
            if (!string.IsNullOrEmpty(query))
            {
                //Get the top 3 from collections (catefories)
                ViewBag.CollectionsData = _context.Categories.OrderBy(s => s.ID).Take(3);


                TempData["SearchText"] = query;

                ViewBag.TotalRecords = _context.Products.Count(s => s.ProductName.Contains(query) || s.ProductDescription.Contains(query) || s.ProductTags.Contains(query));
                //Get all query products
                var data = _context.Products.Where(s => s.ProductName.Contains(query) || s.ProductDescription.Contains(query) || s.ProductTags.Contains(query)).OrderByDescending(s => s.ID).ToListAsync();
                return View(await data);
            }

           return RedirectToAction("", "Shop");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
