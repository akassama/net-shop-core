using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    [TypeFilter(typeof(SessionAuthorize))] 
    public class AccountController : Controller
    {
        AppFunctions functions = new AppFunctions();

        private readonly DBConnection _context;
        private readonly SessionManager _sessionManager;

        public AccountController(SessionManager sessionManager, DBConnection context)
        {
            _sessionManager = sessionManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewPost()
        {

            ViewBag.CurrencyList = functions.GetCurrencyList();
            ViewBag.CategoryList = functions.GetCategoryList();
            ViewBag.StoresList = functions.GetStoresList(_sessionManager.LoginAccountId);

            return View();
        }        
        
        public IActionResult ManagePosts()
        {
            return View();
        }

        public IActionResult TestUpload()
        {
            return View();
        }

        public IActionResult ManageStores()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult Empty()
        {
            return View();
        }





    }
}