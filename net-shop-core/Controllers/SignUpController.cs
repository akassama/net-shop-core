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
                //verify password match
                string RepeatPassword = Request.Form["RepeatPassword"];
                if (!functions.PasswordsMatch(accountsModel.Password, RepeatPassword))
                {
                    TempData["ErrorMessage"] = "Passwords do not match";
                    return View(accountsModel);
                }


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
                TempData["SuccessMessage"] = "Registration was successful";
                return RedirectToAction("Index", "SignIn");
            }
            TempData["ErrorMessage"] = "There was an error processing your request. Please try again. If this error persists, please send an email.";
            return View(accountsModel);
        }

    }
}