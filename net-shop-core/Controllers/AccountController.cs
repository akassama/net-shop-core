using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    [TypeFilter(typeof(SessionAuthorize))] 
    public class AccountController : Controller
    {
        AppFunctions functions = new AppFunctions();

        private readonly DBConnection _context;
        private readonly SessionManager _sessionManager;
        private readonly SystemConfiguration _systemConfiguration;

        public AccountController(SessionManager sessionManager, DBConnection context, IOptions<SystemConfiguration> systemConfiguration)
        {
            _sessionManager = sessionManager;
            _context = context;
            _systemConfiguration = systemConfiguration.Value;
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
            //Get all user stores 
            var data = _context.Stores.Where(s=> s.AccountID == _sessionManager.LoginAccountId).OrderByDescending(s => s.ID).ToList();
            return View(data);
        }
        public IActionResult NewStore()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewStore(StoresModel storesModel)
        {
            if (ModelState.IsValid)
            {
                storesModel.AccountID = _sessionManager.LoginAccountId;
                storesModel.DateAdded = DateTime.Now;

                _context.Add(storesModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Store added successfully";
                return RedirectToAction("ManageStores", "Account");
            }
            TempData["ErrorMessage"] = "Failed to add store";
            return View(storesModel);
        }

        // GET: Products/EditStore/5
        public async Task<IActionResult> EditStore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storesModel = await _context.Stores.FindAsync(id);
            if (storesModel == null)
            {
                return NotFound();
            }
            return View(storesModel);
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