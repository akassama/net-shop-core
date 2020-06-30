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
using Microsoft.Extensions.Options;

namespace ModestLiving.Controllers
{
    public class ShopController : Controller
    {
        private readonly DBConnection _context;

        AppFunctions functions = new AppFunctions();

        private readonly IDetectionService _detectionService;

        private static IHttpContextAccessor _accessor;

        private readonly SystemConfiguration _systemConfiguration;


        public ShopController(DBConnection context, IDetectionService detectionService, IHttpContextAccessor accessor, IOptions<SystemConfiguration> systemConfiguration)
        {
            _accessor = accessor;
            _context = context;
            _detectionService = detectionService;
            _systemConfiguration = systemConfiguration.Value;
        }

        //index view for shop
        public IActionResult Index([FromQuery(Name = "l")] string latest, int page = 0, [FromQuery(Name = "sort")] string sort ="relevance")
        {
            //Get the top 3 from collections (catefories)
            ViewBag.CollectionsData = _context.Categories.OrderBy(s => s.ID).Take(3);

            //set viewbag for sort
            ViewBag.Sort = sort;

            //Get products from db
            var dataSource = _context.Products.Where(s => s.ApproveStatus == 1).OrderByDescending(s => s.ID);

            TempData["LatestCategory"] = "All";
            //If sort by latest category
            if (!string.IsNullOrEmpty(latest) && latest.ToLower() != "all")
            {
                //get category id from category name
                int category_id = _context.Categories.Where(s => s.CategoryName == latest).FirstOrDefault().ID;

                dataSource = _context.Products.Where(s => s.CategoryID == category_id && s.ApproveStatus == 1).OrderByDescending(s => s.ID);

                TempData["LatestCategory"] = latest;
            }

            //check if sort by is selected and sort
            switch (sort)
            {
                case "relevance":
                    dataSource = dataSource.OrderByDescending(s => s.ID);
                    break;
                case "az":
                    dataSource = dataSource.OrderBy(s => s.ProductName);
                    break;
                case "za":
                    dataSource = dataSource.OrderByDescending(s => s.ProductName);
                    break;
                case "lh":
                    dataSource = dataSource.OrderBy(s => s.ProductPrice);
                    break;
                case "hl":
                    dataSource = dataSource.OrderByDescending(s => s.ProductPrice);
                    break;
                default:
                    break;
            }

            //Page size from config file
            int PageSize = _systemConfiguration.shopPageSize;

            int count = dataSource.Count();

            ViewBag.TotalProducts = count;

            var data = dataSource.Skip(page * PageSize).Take(PageSize).ToList();

            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;

            return View(data);

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
        public IActionResult Search([FromQuery(Name = "q")] string query, int page = 0)
        {
            //If search query passed
            if (!string.IsNullOrEmpty(query))
            {
                //Get the top 3 from collections (catefories)
                ViewBag.CollectionsData = _context.Categories.OrderBy(s => s.ID).Take(3);


                TempData["SearchText"] = query;

                ViewBag.TotalRecords = _context.Products.Count(s => s.ProductName.Contains(query) || s.ProductDescription.Contains(query) || s.ProductTags.Contains(query));
                
                //Get all query products
                var dataSource = _context.Products.Where(s => s.ProductName.Contains(query) || s.ProductDescription.Contains(query) || s.ProductTags.Contains(query)).OrderByDescending(s => s.ID);

                //Page size from config file
                int PageSize = _systemConfiguration.searchPageSize;

                int count = dataSource.Count();

                var data = dataSource.Skip(page * PageSize).Take(PageSize).ToList();

                ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;

                return View(data);
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
