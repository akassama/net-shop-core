using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ModestLiving.Controllers
{
    public class AccountStoreController : Controller
    {
        public IActionResult Index()
        {
            TempData["authenticated"] = true;

            return View();
        }

        public IActionResult NewPost()
        {
            TempData["authenticated"] = true;

            return View();
        }        
        
        public IActionResult ManagePosts()
        {
            TempData["authenticated"] = true;

            return View();
        }

        public IActionResult TestUpload()
        {
            TempData["authenticated"] = true;

            return View();
        }
    }
}