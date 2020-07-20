using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    [TypeFilter(typeof(SessionAuthorize))] 
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewPost()
        {
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