using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_shop_core.Models;
using Wangkanai.Detection.Services;

namespace ModestLiving.Controllers
{
    public class TestController : Controller
    {
        private readonly DBConnection _context;
        AppFunctions functions = new AppFunctions();

        private readonly IDetectionService _detectionService;
        private static IHttpContextAccessor _accessor;
        //Session manager ojbect
        SessionManager sessions = new SessionManager(_accessor);

        //const string SessionViewedProducts = "_ViewedProducts";


        public TestController(DBConnection context, IDetectionService detectionService, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _context = context;
            _detectionService = detectionService;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("SessionName", "Laiman");
            var SessionName = HttpContext.Session.GetString("SessionName");

            ViewBag.device = _detectionService.Device.Type;
            ViewBag.browser = _detectionService.Browser.Name;
            ViewBag.platform = _detectionService.Platform.Name;
            ViewBag.engine = _detectionService.Engine.Name;
            var visitor_ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            ViewBag.ip = functions.FormatVisitorIP(visitor_ip);
            ViewBag.country = functions.GetIpInfo("188.130.155.151", "Country");
            ViewBag.session = SessionName;

            int product_id = 2;

            //Get current ViewedProducts session
            var ViewedProducts = (HttpContext.Session.GetString("ViewedProducts") != null) ? HttpContext.Session.GetString("ViewedProducts") : "";

            //if not viewed already
            if (!functions.IsProductViewed(product_id, ViewedProducts))
            {
                //if not null set to session data else set to empty
                HttpContext.Session.SetString("ViewedProducts", ViewedProducts + ",product_id_" + product_id.ToString());
                ViewBag.view = "Logged View";
            }
            else
            {
                ViewBag.view = "Not Logged";
            }

            ViewBag.cartData = HttpContext.Session.GetString("ShoppingCart"); 

            return View(_detectionService);
        }
    }
}