using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_shop_core.Models;
using Microsoft.Extensions.Options;

namespace ModestLiving.Controllers
{
    public class NewArrivalsController : Controller
    {
        private readonly DBConnection _context;
        private readonly SystemConfiguration _systemConfiguration;


        public NewArrivalsController(DBConnection context, IOptions<SystemConfiguration> systemConfiguration)
        {
            _context = context;
            _systemConfiguration = systemConfiguration.Value;
        }

        public IActionResult Index(int page = 0)
        {
            //Get the last 6 products from db
            var dataSource = _context.Products.Where(s => s.ApproveStatus == 1).OrderByDescending(s => s.ID);

            //Page size from config file
            int PageSize = _systemConfiguration.newArrivalPageSize; 

            int count = dataSource.Count();

            var data = dataSource.Skip(page * PageSize).Take(PageSize).ToList();

            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;

            return View(data);
        }
    }
}