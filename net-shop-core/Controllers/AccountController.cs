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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ModestLiving.Controllers
{
    [TypeFilter(typeof(SessionAuthorize))]
    public class AccountController : Controller
    {
        AppFunctions functions = new AppFunctions();

        private readonly DBConnection _context;
        private readonly SessionManager _sessionManager;
        private readonly SystemConfiguration _systemConfiguration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SessionManager sessionManager, DBConnection context, IOptions<SystemConfiguration> systemConfiguration, ILogger<AccountController> logger)
        {
            _sessionManager = sessionManager;
            _context = context;
            _systemConfiguration = systemConfiguration.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            string AccountID = _sessionManager.LoginAccountId;

            //Get total products for user
            ViewBag.TotalProducts = _context.Products.Where(s => s.AccountID == _sessionManager.LoginAccountId).Count();

            return View();
        }


        public IActionResult ManagePosts()
        {
            string AccountID = _sessionManager.LoginAccountId;

            //Get all user posts 
            var data = _context.Products.Where(s => s.AccountID == _sessionManager.LoginAccountId).OrderByDescending(s => s.ID).ToList();
            return View(data);
        }


        public IActionResult NewPost()
        {
            string AccountID = _sessionManager.LoginAccountId;

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
            string AccountID = _sessionManager.LoginAccountId;

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
                    productsModel.FeaturedPost = (string.IsNullOrEmpty(HttpContext.Request.Form["FeaturedPost"])) ? 0 : functions.Int32Parse(HttpContext.Request.Form["FeaturedPost"]);
                    productsModel.ApproveStatus = _systemConfiguration.defaultProductApproveStatus;
                    productsModel.UpdatedBy = _sessionManager.LoginUsername;
                    productsModel.UpdateDate = DateTime.Now;
                    productsModel.DateAdded = DateTime.Now;


                    if(!string.IsNullOrEmpty(HttpContext.Request.Form["EditPost"]) && HttpContext.Request.Form["EditPost"] == "True")
                    {

                        //update post
                        productsModel.ID = functions.Int32Parse(HttpContext.Request.Form["ID"]);
                        productsModel.ProductID = HttpContext.Request.Form["ProductID"];

                        _context.Update(productsModel);
                        await _context.SaveChangesAsync();

                        //remove current post images
                        functions.DeleteProductImages(AccountID, productsModel.ProductID);
                        functions.DeleteTableData("ProductImages", "ProductID", productsModel.ProductID, _systemConfiguration.connectionString);

                    }
                    else
                    {
                        //add post
                        _context.Add(productsModel);
                        await _context.SaveChangesAsync();

                        //add product stock
                        functions.AddTableData("ProductStock", "ProductID", _systemConfiguration.defaultProductStock.ToString(), _systemConfiguration.connectionString);
                    }


                    //Image watermark from config file
                    string TextWaterMark = _systemConfiguration.textWaterMark;
                    string ImageWaterMark = _systemConfiguration.imageWatermark;
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
                                    string NewFileName = functions.RandomString(4) + "-" + file.FileName;
                                    if (!string.IsNullOrEmpty(ImageWaterMark))
                                    {
                                        img.ScaleAndCrop(ImageWidth, ImageHeight)
                                        .AddImageWatermark(@"wwwroot\files\images\" + ImageWaterMark)
                                        .AddTextWatermark(TextWaterMark)
                                        .SaveAs(SavePath + "\\" + NewFileName);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            img.ScaleAndCrop(ImageWidth, ImageHeight)
                                            .AddTextWatermark(TextWaterMark)
                                            .SaveAs(SavePath + "\\" + NewFileName);
                                        }
                                        catch (Exception)
                                        {
                                            //exclude watermark
                                            img.ScaleAndCrop(ImageWidth, ImageHeight)
                                            .SaveAs(SavePath + "\\" + NewFileName);
                                        }
                                    }

                                    //Add image to ProductImages table
                                    functions.AddProductImages(productsModel.ProductID, NewFileName, null);
                                    TotalUploads++;
                                }
                            }
                        }
                    }

                    //Add product video (if added)
                    var ProductVideo = Request.Form["ProductVideo"];
                    if (!string.IsNullOrEmpty(ProductVideo.ToString()))
                    {
                        string NewFileName = functions.RandomString(4) + "-" + ProductVideo;

                        //Add to ProductVideo table
                        functions.AddProductVideo(productsModel.ProductID, NewFileName, null);
                    }

                    TempData["SuccessMessage"] = @$"Product added successfully.  {TotalUploads} images uploaded. 
                        <br/> Add product details here: <a href='/Account/AddProductColors/{productsModel.ProductID}' class='mr-2'>Product Colors</a>
                        <a href='/Account/AddProductSizes/{productsModel.ProductID}'>Product Sizes</a>";

                    return RedirectToAction("ManagePosts", "Account");
                }
                catch (Exception ex)
                {
                    //Log Error
                    _logger.LogInformation("Add Product Error: " + ex.ToString());

                    TempData["ErrorMessage"] = "An error occured while processing your request.";

                    if (!string.IsNullOrEmpty(HttpContext.Request.Form["EditPost"]) && HttpContext.Request.Form["EditPost"] == "True")
                    {
                        return RedirectToAction("EditPost", "Account", new { id = HttpContext.Request.Form["ID"] });
                    }
                   return View(productsModel);
                }
            }

            TempData["ErrorMessage"] = "Failed to add/update product. Missing required input(s).";

            if (!string.IsNullOrEmpty(HttpContext.Request.Form["EditPost"]) && HttpContext.Request.Form["EditPost"] == "True")
            {
                return RedirectToAction("EditPost", "Account", new { id = HttpContext.Request.Form["ID"] });
            }
            return View(productsModel);
        }


        // GET: Account/EditPost/id
        public async Task<IActionResult> EditPost(string id)
        {
            string AccountID = _sessionManager.LoginAccountId;

            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productModel == null)
            {
                return NotFound();
            }

            ViewBag.CurrencyList = functions.GetCurrencyList();
            ViewBag.CategoryList = functions.GetCategoryList();
            ViewBag.StoresList = functions.GetStoresList(_sessionManager.LoginAccountId);


            return View(productModel);
        }


        // GET: Account/AddProductColors/id
        public async Task<IActionResult> AddProductColors(string id)
        {
            string AccountID = _sessionManager.LoginAccountId;

            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST: Account/AddProductColors
        public IActionResult AddProductColors()
        {
            string AccountID = _sessionManager.LoginAccountId;
            string ProductColor = HttpContext.Request.Form["ProductColor"];
            string ProductID = HttpContext.Request.Form["ProductID"];
            try
            {
                string[] ValidationInputs = { ProductColor, ProductID };
                if (!functions.ValidateInputs(ValidationInputs))
                {
                    TempData["ErrorMessage"] = "Validation error. Missing required field(s).";
                    return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
                }

                string ProductColorCode = ProductColor.Replace("#", "");
                string Url = @$"https://www.thecolorapi.com/id?hex={ProductColorCode}";

                var Request = WebRequest.Create(Url);
                using (WebResponse wrs = Request.GetResponse())
                using (Stream stream = wrs.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);

                    string ProductColorName = (string)obj["name"]["value"];

                    functions.AddProductColors(ProductID, ProductColorName, ProductColor);

                    TempData["SuccessMessage"] = "Color '"+ ProductColorName + "' added.";
                    return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
                }

            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Add Product Color Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                if (!string.IsNullOrEmpty(ProductID))
                {
                    return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
                }
                return RedirectToAction("ManagePosts", "Account");
            }
        }





        // GET: Account/AddProductSizes/id
        public async Task<IActionResult> AddProductSizes(string id)
        {
            string AccountID = _sessionManager.LoginAccountId;

            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST: Account/AddProductSizes
        public IActionResult AddProductSizes()
        {
            string ProductID = HttpContext.Request.Form["ProductID"];
            string ProductSizeCountry = HttpContext.Request.Form["ProductSizeCountry"];
            string ProductSizeType = HttpContext.Request.Form["ProductSizeType"];
            string ProductSize = HttpContext.Request.Form["ProductSize"];

            try
            {
                string[] ValidationInputs = { ProductSizeCountry, ProductSizeType, ProductSize };
                if (!functions.ValidateInputs(ValidationInputs))
                {
                    TempData["ErrorMessage"] = "Validation error. Missing required field(s).";
                    return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
                }

                string SizeData = $@"{ProductSizeCountry},{ProductSizeType},{ProductSize}";
                functions.AddProductSizes(ProductID, SizeData);

                TempData["SuccessMessage"] = "Size added.";
                return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Add Product Size Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                if (!string.IsNullOrEmpty(ProductID))
                {
                    return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
                }
                return RedirectToAction("ManagePosts", "Account");
            }
        }




        // POST: Account/DeletePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct()
        {
            string AccountID = _sessionManager.LoginAccountId;

            string RemoveProductID = HttpContext.Request.Form["ModalDeleteProductID"];


            string[] ValidationInputs = { RemoveProductID };
            if (!functions.ValidateInputs(ValidationInputs))
            {
                TempData["ErrorMessage"] = "Validation error. Missing required field(s).";

                return RedirectToAction("ManagePosts", "Account");
            }

            try
            {
                string PostTitle = _context.Products.Where(s => s.ProductID == RemoveProductID).FirstOrDefault().ProductName;

                //remove account
                functions.RemoveProduct(RemoveProductID, _systemConfiguration.connectionString);

                //log activity
                if (_systemConfiguration.logActivity)
                {
                    string LogAction = $@"Product with name '{PostTitle}' has been removed by {functions.GetAccountData(AccountID, "FullName")}";
                    functions.LogActivity(AccountID, AccountID, "RemovePost", LogAction);
                }

                TempData["SuccessMessage"] = "Product removed.";
                return RedirectToAction("ManagePosts", "Account");
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Remove Product Error: " + ex.ToString());

                TempData["ErrorMessage"] = "There was an error processing your request. Please try again. If this error persists, please send an email to the administrator.";
                return RedirectToAction("ManagePosts", "Account");
            }
        }



        // GET: Account/UpdateProductStock/id
        public async Task<IActionResult> UpdateProductStock(string id)
        {
            string AccountID = _sessionManager.LoginAccountId;

            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST: Account/AddProductSizes
        public IActionResult UpdateProductStock()
        {
            string ProductID = HttpContext.Request.Form["ProductID"];
            string NumberInStock = HttpContext.Request.Form["NumberInStock"];

            try
            {
                string[] ValidationInputs = { NumberInStock};
                if (!functions.ValidateInputs(ValidationInputs))
                {
                    TempData["ErrorMessage"] = "Validation error. Missing required field(s).";
                    return RedirectToAction("UpdateProductStock", "Account", new { id = ProductID });
                }

               
                var DBQuery = _context.ProductStock.Where(s => s.ProductID == ProductID);
                
                if (!DBQuery.Any())
                {
                    //if stock does not exist, add
                    functions.AddTableData("ProductStock", "ProductID", ProductID, _systemConfiguration.connectionString);
                    functions.UpdateTableData("ProductStock", "ProductID", ProductID, "NumberInStock", NumberInStock, _systemConfiguration.connectionString);
                }
                else
                {
                    //if stock exist, update
                    functions.UpdateTableData("ProductStock", "ProductID", ProductID, "NumberInStock", NumberInStock, _systemConfiguration.connectionString);
                }

                TempData["SuccessMessage"] = "Stock updated.";
                return RedirectToAction("UpdateProductStock", "Account", new { id = ProductID });
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Add Product Stock Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                if (!string.IsNullOrEmpty(ProductID))
                {
                    return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
                }
                return RedirectToAction("ManagePosts", "Account");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST: Account/ResetProductColors
        public IActionResult ResetProductColors()
        {
            string ProductID = HttpContext.Request.Form["ModalResetColorID"];

            try
            {
                string[] ValidationInputs = { ProductID };
                if (!functions.ValidateInputs(ValidationInputs))
                {
                    TempData["ErrorMessage"] = "Validation error. Missing required field(s).";
                    return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
                }

                functions.DeleteTableData("ProductColors", "ProductID", ProductID, _systemConfiguration.connectionString);

                TempData["SuccessMessage"] = "Product colors have been resseted/removed.";

                return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Reset Product Colors Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                if (!string.IsNullOrEmpty(ProductID))
                {
                    return RedirectToAction("AddProductColors", "Account", new { id = ProductID });
                }
                return RedirectToAction("ManagePosts", "Account");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST: Account/ResetProductColors
        public IActionResult ResetProductSizes()
        {
            string ProductID = HttpContext.Request.Form["ModalResetSizeID"];

            try
            {
                string[] ValidationInputs = { ProductID };
                if (!functions.ValidateInputs(ValidationInputs))
                {
                    TempData["ErrorMessage"] = "Validation error. Missing required field(s).";
                    return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
                }

                functions.DeleteTableData("ProductSizes", "ProductID", ProductID, _systemConfiguration.connectionString);

                TempData["SuccessMessage"] = "Product sizes have been resseted/removed.";

                return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Reset Product Size Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                if (!string.IsNullOrEmpty(ProductID))
                {
                    return RedirectToAction("AddProductSizes", "Account", new { id = ProductID });
                }
                return RedirectToAction("ManagePosts", "Account");
            }
        }


        //TODO
        // GET: Account/ViewProduct/id
        public async Task<IActionResult> ViewProduct(string id)
        {
            string AccountID = _sessionManager.LoginAccountId;

            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
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
                catch (Exception ex)
                {
                    //Log Error
                    _logger.LogInformation("Add Store Error: " + ex.ToString());

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

        public async Task<IActionResult> Profile([FromQuery(Name = "edit")] string edit)
        {
            string AccountID = _sessionManager.LoginAccountId;

            //get connection string from app settings
            ViewBag.ConnectionString = _systemConfiguration.connectionString;
            //Get countries list
            ViewBag.CountriesList = functions.GetCountryList();

            int id = _context.Accounts.Where(s => s.AccountID == AccountID).FirstOrDefault().ID;

            var accountModel = await _context.Accounts.FindAsync(id);
            if (accountModel == null)
            {
                return NotFound();
            }

            //checks if edit. Enable/Disable inputs and Update button
            ViewBag.EditProfile = edit;

            if (!string.IsNullOrEmpty(edit) && edit == "true")
            {
                ViewBag.EditProfile = "true";
            }

            return View(accountModel);
        }

        // POST: Account/Profile/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AccountsModel accountsModel)
        {
            //get connection string from app settings
            ViewBag.ConnectionString = _systemConfiguration.connectionString;
            //Get countries list
            ViewBag.CountriesList = functions.GetCountryList();

            try
            {
                int id = _context.Accounts.Where(s => s.AccountID == accountsModel.AccountID).FirstOrDefault().ID;
                if (id != accountsModel.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        //Set post image to empty. Assigned upon upload
                        string ProfileImage = null;
                        //Upload profile image if uploaded
                        //Image dimention
                        int ImageHeight = 150;
                        int ImageWidth = 150;

                        //check if PostImage in model
                        if (Request.Form.Files.Count > 0)
                        {
                            //Saving file with resize, text and image watermark
                            var DirectoryName = _sessionManager.LoginDirectoryName;
                            var SavePath = @"wwwroot\\files\\" + DirectoryName + "\\";

                            //create directory if not exist
                            Directory.CreateDirectory(SavePath);

                            foreach (var file in Request.Form.Files)
                            {
                                if (file.Length > 0)
                                {
                                    using (var stream = file.OpenReadStream())
                                    {
                                        using (var img = Image.FromStream(stream))
                                        {
                                            string NewFileName = functions.RandomString(8) + "-" + file.FileName;

                                            img.ScaleAndCrop(ImageWidth, ImageHeight)
                                                .SaveAs(SavePath + "\\" + NewFileName);

                                            //Set profile image
                                            ProfileImage = NewFileName;
                                        }
                                    }
                                }
                            }
                        }

                        accountsModel.ProfilePicture = ProfileImage;

                        _context.Update(accountsModel);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AccountsModelExists(accountsModel.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["SuccessMessage"] = "Profile updated.";
                    return RedirectToAction(nameof(Index));
                }
                return View(accountsModel);
            }
            catch (Exception ex)
            {
                //Log Error
                _logger.LogInformation("Edit Profile Error: " + ex.ToString());

                TempData["ErrorMessage"] = "An error occured while processing your request.";
                return View(accountsModel);
            }
        }

        private bool AccountsModelExists(int id)
        {
            return _context.Accounts.Any(e => e.ID == id);
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