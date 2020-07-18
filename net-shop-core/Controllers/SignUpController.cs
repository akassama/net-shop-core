using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net_shop_core.Models;

namespace net_shop_core.Controllers
{
    public class SignUpController : Controller 
    {
        AppFunctions functions = new AppFunctions();

        private readonly DBConnection _context;
        public SignUpController(DBConnection context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountsModel accountsModel)
        {
            if (ModelState.IsValid)
            {
                accountsModel.Password = BCrypt.Net.BCrypt.HashPassword(accountsModel.Password);
                //Default registration values
                accountsModel.AccountID = functions.GetUinqueId();
                accountsModel.DirectoryName = functions.GenerateDirectoryName(accountsModel.Email);
                accountsModel.Oauth = 0;
                accountsModel.AccountVerification = 0;
                accountsModel.Status = 0;
                accountsModel.UpdateDate = DateTime.Now;

                _context.Add(accountsModel);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Registration was successful";
                return RedirectToAction("Index", "SignIn");
            }
            TempData["Message"] = "Registration Failed";
            return View(accountsModel);
        }

    }
}