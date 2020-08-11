using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using net_shop_core.Models;

using System.Drawing;
using LazZiya.ImageResize;

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
            //Get total products for user
            ViewBag.TotalProducts = _context.Products.Where(s => s.AccountID == _sessionManager.LoginAccountId).Count();

            return View();
        }

        public IActionResult NewPost()
        {

            ViewBag.CurrencyList = functions.GetCurrencyList();
            ViewBag.CategoryList = functions.GetCategoryList();
            ViewBag.StoresList = functions.GetStoresList(_sessionManager.LoginAccountId);

            return View();
        }

        // POST: Account/NewPost/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPost(ProductsModel productsModel) 
        {
            //Set ViewBags data for form return data
            ViewBag.CurrencyList = functions.GetCurrencyList();
            ViewBag.CategoryList = functions.GetCategoryList();
            ViewBag.StoresList = functions.GetStoresList(_sessionManager.LoginAccountId);

            if (ModelState.IsValid)
            {
                try
                {
                    //Set other product data
                    productsModel.ProductID = functions.GetUinqueId();
                    productsModel.AccountID = _sessionManager.LoginAccountId;
                    productsModel.UniqueProductName = functions.GenerateUniqueProductName(productsModel.ProductName);
                    productsModel.ApproveStatus = _systemConfiguration.defaultProductApproveStatus; 
                    productsModel.UpdatedBy = _sessionManager.LoginUsername;
                    productsModel.UpdateDate = DateTime.Now;
                    productsModel.DateAdded = DateTime.Now;


                    _context.Add(productsModel);
                    await _context.SaveChangesAsync();

                    //Image watermark from config file
                    string TextWaterMark = _systemConfiguration.textWaterMark;
                    string ImageWaterMark = _systemConfiguration.imageWaterMark;
                    int ImageHeight = _systemConfiguration.uploadImageDefaultHeight;
                    int ImageWidth = _systemConfiguration.uploadImageDefaultWidth;

                    //Get account directory name
                    var DirectoryName = functions.GetAccountData(_sessionManager.LoginAccountId, "DirectoryName");
                    var SavePath = @"wwwroot\\files\\" + DirectoryName + "\\products";

                    int TotalUploads = 0;
                    //Loop through files and upload
                    foreach (var file in Request.Form.Files)
                    {
                        if (file.Length > 0)
                        {
                            using (var stream = file.OpenReadStream())
                            {
                                using (var img = Image.FromStream(stream))
                                {
                                    string NewFileName = file.FileName + "-" + functions.RandomString(4);
                                    if (!string.IsNullOrEmpty(ImageWaterMark))
                                    {
                                        img.ScaleAndCrop(340, 600)
                                        .AddImageWatermark(@"wwwroot\files\images\"+ImageWaterMark)
                                        .AddTextWatermark(TextWaterMark)
                                        .SaveAs(SavePath + "\\" + file.FileName);
                                    }
                                    else
                                    {
                                        img.ScaleAndCrop(ImageWidth, ImageHeight)
                                        .AddTextWatermark(TextWaterMark)
                                        .SaveAs(SavePath + "\\" + file.FileName);
                                    }

                                    //Add image to ProductImages table
                                    functions.AddProductImages(productsModel.ProductID, file.FileName, null);
                                    TotalUploads++;
                                }
                            }
                        }
                    }

                    //Add product colors
                    var ProductColors = Request.Form["ProductColors"];
                    if (!string.IsNullOrEmpty(ProductColors.ToString()))
                    {
                        foreach (var item in ProductColors)
                        {
                            //Add to ProductColors table
                            functions.AddProductColors(productsModel.ProductID, item);
                        }
                    }

                    //Add product sizes
                    var ProductSizes = Request.Form["ProductSizes"];
                    if (!string.IsNullOrEmpty(ProductSizes.ToString()))
                    {
                        foreach (var item in ProductSizes)
                        {
                            //Add to ProductSizes table
                            functions.AddProductSizes(productsModel.ProductID, item);
                        }
                    }

                    TempData["SuccessMessage"] = "Product added successfully. " + TotalUploads + " images uploaded.";
                    return RedirectToAction("ManagePosts", "Account");
                }
                catch (Exception ex)
                {
                    //TODO log error
                    TempData["ErrorMessage"] = "An error occured while processing your request.";
                    return View(productsModel);
                }
            }
            TempData["ErrorMessage"] = "Failed to add product";

            return View(productsModel);
        }

        public IActionResult ManagePosts()
        {
            //Get all user posts 
            var data = _context.Products.Where(s => s.AccountID == _sessionManager.LoginAccountId).OrderByDescending(s => s.ID).ToList();
            return View(data);
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

        // POST: Account/NewStore/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewStore(StoresModel storesModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    storesModel.AccountID = _sessionManager.LoginAccountId;
                    storesModel.DateAdded = DateTime.Now;

                    _context.Add(storesModel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Store added successfully";
                }
                catch (Exception)
                {
                    //TODO log error
                    TempData["ErrorMessage"] = "An error occured while processing your request.";
                    return View(storesModel);
                }

                return RedirectToAction("ManageStores", "Account");
            }
            TempData["ErrorMessage"] = "Failed to add store";
            return View(storesModel);
        }

        // GET: Account/EditStore/5
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