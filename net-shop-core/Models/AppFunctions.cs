using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using net_shop_core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace net_shop_core.Models
{
    public class AppFunctions
    {
        const string SessionViewedProducts = "_ViewedProducts";

        //Log product view to database
        public bool LogProductView(string product_id, string visitor_id, string visitor_ip, string visitor_browser, string visitor_device, string other)
        {

            using (var db = new DBConnection())
            {
                // Create product object.
                ProductViewsModel product = new ProductViewsModel
                {
                    ProductID = product_id,
                    VisitorID = visitor_id,
                    IpAddress = visitor_ip,
                    Country = GetIpInfo(visitor_ip, "Country"),
                    Browser = visitor_browser,
                    Device = visitor_device, 
                    VisitDate = DateTime.Now,
                    OtherDetails = other
                };

                // Add the new object to the db collection.
                db.ProductViews.Add(product);

                // Submit the change to the database.
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    //TODO Log error
                    Console.WriteLine(ex);
                }
                return false;
            }
        }


        //Get current visitor ip address
        /// <summary>
        /// Get current user ip address.
        /// </summary>
        /// <returns>The IP Address</returns>
        public string FormatVisitorIP(string ip_address)
        {
            // Get the IP  
            if (ip_address == "::1")
            {
                ip_address = "127.0.0.1";
            }
            return ip_address;
        }


        //Get country info
        /// <summary>
        /// Get current visitor country info based on ip address
        /// </summary>
        /// <returns>The the info for the second parameter passed</returns>
        public string GetIpInfo(string ip_address, string return_data)
        {
            string url = "http://api.ipstack.com/" + ip_address + "?access_key=49826d840276f76c7e91ba2ce5c1a700";
            var request = WebRequest.Create(url);

            using (WebResponse wrs = request.GetResponse())
            using (Stream stream = wrs.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var obj = JObject.Parse(json);
                string continent = (string)obj["continent_name"];
                string city = (string)obj["city"];
                string country = (string)obj["country_name"];
                string country_code = (string)obj["country_code"];
                var country_flag_url = (string)obj["location"]["country_flag"];
                string calling_code = (string)obj["location"]["calling_code"];

                switch (return_data)
                {
                    case "Continent":
                        return continent;
                    case "Country":
                        return country;
                    case "City":
                        return city;
                    case "CountryCode":
                        return country_code;
                    case "Flag":
                        return country_flag_url;
                    case "CallCode":
                        return calling_code;
                    default:
                        return null;
                }
            }
        }

        //Checks if product is viewed
        /// <summary>
        /// Check if the product id has already been viewed by the visitor
        /// </summary>
        /// <returns>True or False</returns>
        public bool IsProductViewed(string product_id, string _viewed_products)
        {
            var viewed_products = "";
            if (_viewed_products != null)
            {
                viewed_products = _viewed_products;
            }

            //if not viewed already
            if (!viewed_products.Contains("product_id_" + product_id.ToString()))
            {
                return false;
            }

            return true;
        }


        //Generate unique id
        /// <summary>
        /// Generate unique directory name for new account created, takes in user email
        /// </summary>
        /// <returns>unique directory name</returns>
        public string GenerateDirectoryName(string user_email)
        {
            if (user_email.Contains("@"))
            {
                user_email = user_email.Split('@')[0];
            }
            return (user_email + RandomString(8)).ToLower();
        }


        //Generates unique alphanumeric strings
        /// <summary>
        /// Generate unique alphanumeric strings
        /// </summary>
        /// <returns>unique string</returns>
        public string GetUinqueId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            return FormNumber.ToString("X");

        }

        //Generates random alphanumeric strings
        /// <summary>
        /// Take in the length and generates random alphanumeric string
        /// </summary>
        /// <returns>alphanumeric string</returns>
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        //Get List Of Currencies 
        /// <summary>
        /// Get the list of currencies in database
        /// </summary>
        /// <returns>list of currencies </returns>
        public List<CurrencyModel> GetCurrencyList()
        {
            using (var db = new DBConnection())
            {
                List<CurrencyModel> currency_list = new List<CurrencyModel>();

                //-- Get data from db --//
                currency_list = db.Currency.ToList();

                //-- Inserting select item in list --//
                //currency_list.Insert(0, new CurrencyModel {Code = "Select Currency" });

                return currency_list;
            }
        }

        //Get List Of Product Categories 
        /// <summary>
        /// Get the list of categories in database
        /// </summary>
        /// <returns>list of categories </returns>
        public List<CategoriesModel> GetCategoryList()
        {
            using (var db = new DBConnection())
            {
                List<CategoriesModel> category_list = new List<CategoriesModel>();

                //-- Get data from db --//
                category_list = db.Categories.ToList();

                //-- Inserting select item in list --//
                //category_list.Insert(0, new CategoriesModel { ID = 0, CategoryName = "Select Category" });

                return category_list;
            }
        }


        //Get List Of Stores
        /// <summary>
        /// Get the list of all stores
        /// </summary>
        /// <returns>list of stores</returns>
        public List<StoresModel> GetStoresList()
        {
            using (var db = new DBConnection())
            {
                List<StoresModel> stores_list = new List<StoresModel>();

                //-- Get data from db --//
                stores_list = db.Stores.ToList();

                //-- Inserting select item in list --//
                //stores_list.Insert(0, new StoresModel { ID = 0, StoreName = "Select Store" });

                return stores_list;
            }
        }

        //Get List Of Stores
        /// <summary>
        /// Get the list of store for user
        /// </summary>
        /// <returns>list of stores</returns>
        public List<StoresModel> GetStoresList(string account_id)
        {
            using (var db = new DBConnection())
            {
                List<StoresModel> stores_list = new List<StoresModel>();

                //-- Get data from db --//
                stores_list = db.Stores.Where(s=> s.AccountID == account_id).ToList();

                //-- Inserting select item in list --//
                stores_list.Insert(0, new StoresModel {ID = 0, StoreName = "Select Store" });

                return stores_list;
            }
        }


        //Get Specific Account Data
        /// <summary>
        /// Get the account data passed
        /// </summary>
        /// <returns>account data value</returns>
        public string GetAccountData(string account_id, string account_data)
        {
            using (var db = new DBConnection())
            {
                var data = db.Accounts.Where(s => s.AccountID == account_id);
                switch (account_data)
                {
                    case "FirstName":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.FirstName != null);
                        return (data.Any() ? data.FirstOrDefault().FirstName :  null);
                    case "LastName":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.LastName != null);
                        return (data.Any() ? data.FirstOrDefault().LastName : null);
                    case "FullName":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.FirstName != null && s.LastName != null);
                        return (data.Any() ? data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName : null);
                    case "Country":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.Country != null);
                        return (data.Any() ? data.FirstOrDefault().Country : null);
                    case "CountryCode":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.CountryCode != null);
                        return (data.Any() ? data.FirstOrDefault().CountryCode.ToString() : null);
                    case "PhoneNumber":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.PhoneNumber != null);
                        return (data.Any() ? data.FirstOrDefault().PhoneNumber.ToString() : null);
                    case "Email":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.Email != null);
                        return (data.Any() ? data.FirstOrDefault().Email : null);
                    case "ProfilePicture":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.ProfilePicture != null);
                        return (data.Any() ? data.FirstOrDefault().ProfilePicture : null);
                    case "Status":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.Status != null);
                        return (data.Any() ? data.FirstOrDefault().Status.ToString() : null);
                    case "Oauth":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.Oauth != null);
                        return (data.Any() ? data.FirstOrDefault().Oauth.ToString() : null);
                    case "DirectoryName":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.DirectoryName != null);
                        return (data.Any() ? data.FirstOrDefault().DirectoryName : null);
                    case "DateAdded":
                        data = db.Accounts.Where(s => s.AccountID == account_id && s.DateAdded != null);
                        return (data.Any() ? data.FirstOrDefault().DateAdded.ToString() : null);
                    default:
                        return null;
                }
            }
        }


        //Add image to Product Images table
        /// <summary>
        /// Add product images
        /// </summary>
        /// <returns>boolean</returns>
        public bool AddProductImages(string product_id, string product_image, string description)
        {
            using (var db = new DBConnection())
            {
                // Create ProductImage object.
                ProductImagesModel image_data = new ProductImagesModel
                {
                    ProductID = product_id,
                    ImageType = GetImageFormat(product_image).ToString(),
                    ImageLink = product_image,
                    ImageDescription = description,
                    DateAdded = DateTime.Now
                };

                // Add object to the ProductImages collection.
                db.ProductImages.Add(image_data);

                // Submit the change to the database.
                try
                {
                    db.SaveChanges();
                    return true;

                }
                catch (Exception)
                {
                    //TODO log error
                    return false;
                }
            }
        }


        //Gets image file extension
        /// <summary>
        /// Get the image file extension
        /// </summary>
        /// <returns>image file extension</returns>
        private ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }


        //Generate unique product name
        /// <summary>
        /// Generate unique product name for new product created, takes in product name text
        /// </summary>
        /// <returns>unique product name</returns>
        public string GenerateUniqueProductName(string product_name)
        {
            //remove multiple spaces if any
            product_name = Regex.Replace(product_name, @"\s+", " ");

            //replace all spaces with dash
            product_name = product_name.Replace(' ', '-') + "-" +GetUinqueId();

            return product_name.ToLower();
        }


        //Add colors to Product Colors table
        /// <summary>
        /// Add product colors
        /// </summary>
        /// <returns>boolean</returns>
        public bool AddProductColors(string product_id, string product_color)
        {
            using (var db = new DBConnection())
            {
                // Create ProductColors object.
                ProductColorsModel color_data = new ProductColorsModel
                {
                    ProductID = product_id,
                    ColorName = product_color,
                    ColorCode = product_color,
                    DateAdded = DateTime.Now
                };

                // Add object to the ProductColors collection.
                db.ProductColors.Add(color_data);

                // Submit the change to the database.
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    //TODO log error
                    return false;
                }
            }
        }


        //Add colors to Product Colors table
        /// <summary>
        /// Add product colors
        /// </summary>
        /// <returns>boolean</returns>
        public bool AddProductSizes(string product_id, string product_size)
        {
            using (var db = new DBConnection())
            {
                // Create ProductSizes object.
                ProductSizeModel size_data = new ProductSizeModel
                {
                    ProductID = product_id,
                    Size = product_size,
                    DateAdded = DateTime.Now
                };

                // Add object to the ProductColors collection.
                db.ProductSize.Add(size_data);

                // Submit the change to the database.
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    //TODO log error
                    return false;
                }
            }
        }

    }

}
