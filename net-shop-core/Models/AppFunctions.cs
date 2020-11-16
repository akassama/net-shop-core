using AppHelpers.App_Code;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using net_shop_core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AppHelpers.App_Code.AccountHelper;

namespace net_shop_core.Models
{
    public class AppFunctions
    {
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


        //Converts string to integer
        /// <summary>
        /// Converts string to integer, returns zero if fails
        /// </summary>
        /// <returns>integer</returns>
        public int Int32Parse(string string_number)
        {
            try
            {
                return Int32.Parse(string_number); ;
            }
            catch (FormatException)
            {
                return 0;
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



        //Password match check
        /// <summary>
        /// Checks if the passwords passed are the same
        /// </summary>
        /// <returns>boolean</returns>
        public bool PasswordsMatch(string password_one, string password_two)
        {
            if (password_one.Equals(password_two))
            {
                return true;
            }
            return false;
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
        public bool AddProductColors(string product_id, string color_name, string color_code)
        {
            using (var db = new DBConnection())
            {
                // Create ProductColors object.
                ProductColorsModel color_data = new ProductColorsModel
                {
                    ProductID = product_id,
                    ColorName = color_name,
                    ColorCode = color_code,
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
                ProductSizesModel size_data = new ProductSizesModel
                {
                    ProductID = product_id,
                    Size = product_size,
                    DateAdded = DateTime.Now
                };

                // Add object to the ProductColors collection.
                db.ProductSizes.Add(size_data);

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
        public bool AddProductVideo(string product_id, string product_video_link, string description)
        {
            using (var db = new DBConnection())
            {
                // Create ProductVideos object.
                ProductVideosModel video_data = new ProductVideosModel
                {
                    ProductID = product_id,
                    VideoLink = product_video_link,
                    VideoDescription = description,  
                    DateAdded = DateTime.Now
                };

                // Add object to the ProductVideos collection.
                db.ProductVideos.Add(video_data);

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

        //Validate required inputs 
        /// <summary>
        /// Validates array of inputs for not null
        /// </summary>
        /// <returns>boolean</returns>
        public bool ValidateInputs(string[] inputs)
        {
            // Loop over and check if empty.
            for (int i = 0; i < inputs.Length; i++)
            {
                if (string.IsNullOrEmpty(inputs[i]))
                {
                    return false;
                }
            }
            return true;
        }



        //Remove Product
        /// <summary>
        /// Delete Product Data
        /// </summary>
        /// <returns>boolean</returns>
        public bool RemoveProduct(string product_id, string connection_string)
        {
            using (var db = new DBConnection())
            {
                try
                {
                    // Query the database for the rows to be deleted.
                    var deletePostDetails =
                        from details in db.Products
                        where details.ProductID == product_id
                        select details;

                    foreach (var detail in deletePostDetails)
                    {
                        db.Products.Remove(detail);
                    }

                    db.SaveChanges();

                    //delete relational data
                    DeleteTableData("ProductSizes", "ProductID", product_id, connection_string); //delete from ProductSizes
                    DeleteTableData("ProductColors", "ProductID", product_id, connection_string); //delete from ProductColors
                    DeleteTableData("ProductImages", "ProductID", product_id, connection_string); //delete from ProductImages
                    DeleteTableData("ProductStock", "ProductID", product_id, connection_string); //delete from ProductStock
                    DeleteTableData("ProductVideos", "ProductID", product_id, connection_string); //delete from ProductVideos

                    return true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //TODO Provide for exceptions.
                }
            }
            return false;
        }




        //Add record into table
        /// <summary>
        /// Adds new recored into entity table passed
        /// </summary>
        /// <returns>boolean</returns>
        public bool AddTableData(string model_name, string entry_column, string entry_value, string connection_string)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        //Insert record to Users db
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = @"INSERT INTO [" + model_name + "] ([" + entry_column + "]) VALUES (@value)";
                        cmd.Parameters.AddWithValue("@value", ((object)entry_value) ?? DBNull.Value);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (connection != null)
                        {
                            //cleanup connection i.e close 
                            connection.Close();
                        }

                        if (rowsAffected == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //TODO Log Error
            }

            return false;
        }


        //Update Data Model Record
        /// <summary>
        /// Updates a column value of the data entity passed
        /// </summary>
        /// <returns>boolean</returns>
        public bool UpdateTableData(string model_name, string pk_name, string pk_value, string update_column, string update_value, string connection_string)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    string DBQuery = $"Update [" + model_name + "] SET [" + update_column + "] = '" + update_value + "' Where [" + pk_name + "] = '" + pk_value + "' ";
                    using (SqlCommand command = new SqlCommand(DBQuery, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //TODO Provide for exceptions.
            }
            return false;
        }



        //Delete table record
        /// <summary>
        /// Delete table record(s) base on the key passed
        /// </summary>
        /// <returns>boolean</returns>
        public bool DeleteTableData(string model_name, string pk_name, string pk_value, string connection_string)
        {
            var MsgCountQuery = @"DELETE FROM [" + model_name + "] WHERE [" + pk_name + "]  = @key";
            try
            {
                using (var con = new SqlConnection(connection_string))
                {
                    con.Open();
                    var cmd = new SqlCommand(MsgCountQuery, con);
                    cmd.Parameters.AddWithValue("@key", pk_value);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                //throw; TODO Log error
                return false;
            }
        }


        /// Delete image in directory
        /// <summary>
        ///  Delete image in directory
        /// </summary>
        /// <returns>boolean</returns>
        /// 
        public bool DeleteProductImages(string account_id, string product_id)
        {
            using (var db = new DBConnection())
            {
                // Query the database for the rows to be deleted.
                var DBQuery = db.ProductImages.Where(s => s.ProductID == product_id);
                try
                {
                    foreach (var item in DBQuery)
                    {
                        var FilePath = @"wwwroot\\files\\" + ProductsHelper.GetProductImageLink(account_id, item.ProductID);
                        if (File.Exists(FilePath))
                        {
                            File.Delete(FilePath);
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }



        /// <summary>
        /// Logs activity
        /// </summary>
        public bool LogActivity(string activity_user, string action_by, string log_type, string action)
        {
            using (var db = new DBConnection())
            {
                ActivityLogsModel activity = new ActivityLogsModel
                {
                    ActivityUser = activity_user,
                    ActionBy = action_by,
                    LogType = log_type,
                    Action = action,
                    ActivityDate = DateTime.Now
                    // …
                };

                // Add the new object to the collection.
                db.ActivityLogs.Add(activity);

                // Submit the change to the database.
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return false;
        }


        //Get List Of Countries
        /// <summary>
        /// Get the list of all countries
        /// </summary>
        /// <returns>list of countries</returns>
        public List<CountryModel> GetCountryList()
        {
            using (var db = new DBConnection())
            {
                List<CountryModel> country_list = new List<CountryModel>();

                //-- Get data from db --//
                country_list = db.Countries.OrderBy(s => s.Name).ToList();

                return country_list;
            }
        }


    }

}
