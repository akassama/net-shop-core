using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ModestLiving.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login()
        {
            return RedirectToAction("Index", "AccountStore");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }


    }
}