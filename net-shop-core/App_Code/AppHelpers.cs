using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using net_shop_core.Models;
using System.Globalization;

namespace AppHelpers.App_Code
{

    public static class ListHelper
    {
        //iterates through array and print as html list
        public static HtmlString CreateList(this IHtmlHelper html, string[] items)
        {
            string result = "<ul>";
            foreach (string item in items)
            {
                result = $"{result}<li>{item}</li>";
            }
            result = $"{result}</ul>";
            return new HtmlString(result);
        }
    }

    public static class TextHelper
    {
        //removes html tags from text
        public static string StripHTML(string text)
        {
            text = Regex.Replace(text, "<.*?>", "");
            return text;
        }


        //trims text to the desired lenght passed in the parameter
        public static string FormatLongText(string text, int max_length)
        {
            if (text != null && text.Length > max_length)
            {
                int iNextSpace = text.LastIndexOf(" ", max_length, StringComparison.Ordinal);
                //text = string.Format("{0}...", text.Substring(0, (iNextSpace > 0) ? iNextSpace : max_length).Trim());
                text = $"{(text.Substring(0, (iNextSpace > 0) ? iNextSpace : max_length).Trim())}...";
            }
            return text;
        }

        //convert text case
        public static string ConvertCase(string text, string convert_to)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            switch (convert_to)
            {
                case "Upper":
                    // convert to upper case
                    return textInfo.ToUpper(text);
                case "Lower":
                    // convert to lower case
                    return textInfo.ToLower(text);
                case "Title":
                    // convert to title case
                    return textInfo.ToTitleCase(text);
                case "SplitUpper":
                    //split text by capital case
                    return Regex.Replace(text, "([A-Z])", " $1").Trim();
                default:
                    return text;
            }

        }
    }


    //Helper for account details
    public static class AccountHelper
    {
        //get username directory of product uploader using account id of uploader
        public static string GetProductImageDirectoty(string account_id)
        {
            try
            {
                using (var db = new DBConnection())
                {
                    var query = db.Accounts.Where(s => s.AccountID == account_id);
                    if (query.Any())
                    {
                        return query.FirstOrDefault().DirectoryName;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log error
                Console.WriteLine(ex);
            }
            return null;
        }

        //get account profile picture
        public static string GetAccountProfilePicture(string account_id) 
        {
            try
            {
                using (var db = new DBConnection())
                {
                    var query = db.Accounts.Where(s => s.AccountID == account_id);
                    if (query.Any())
                    {
                        return query.FirstOrDefault().DirectoryName;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log error
                Console.WriteLine(ex);
            }
            return null;
        }
    }

    //Helper for site data 
    public static class SiteDataHelper
    {
        //get home page banner image
        public static string GetSiteLookup(string key)
        {
            using (var db = new DBConnection())
            {
                var query = db.SiteDataLookup.Where(s => s.UinqueKey == key);
                if (query.Any())
                {
                    return query.FirstOrDefault().Value;
                }
            }
            return null;
        }
    }


    //Helper for product data
    public static class ProductsHelper
    {

        //get product category from category id
        public static string GetProductCategory(int category_id)
        {
            try
            {
                using (var db = new DBConnection())
                {
                    var query = db.Categories.Where(s => s.ID == category_id);
                    if (query.Any())
                    {
                        return query.FirstOrDefault().CategoryName;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log error
                Console.WriteLine(ex);
            }
            return "Unknown";
        }

        //get number of products in product category using category id
        public static int GetProductCategoryCount(int category_id)
        {
            try
            {
                using (var db = new DBConnection())
                {
                    return db.Products.Count(s => s.CategoryID == category_id);
                }
            }
            catch(Exception ex)
            {
                //TODO log error
                Console.WriteLine(ex);
                return 0;
            }
        }

        //get product image link using account id of uploader. Takes the first image
        public static string GetProductImageLink(string account_id, string product_id)
        {
            //if user directory name is (for some reason), return place holder image
            var directory_name = AccountHelper.GetProductImageDirectoty(account_id);
            if (directory_name == null)
            {
                return "defaults/products/product-placeholder.jpg";
            }

            using (var db = new DBConnection())
            {
                var query = db.ProductImages.Where(s => s.ProductID == product_id).OrderBy(s=> s.ID).Take(1);
                if (query.Any())
                {
                    return  directory_name + "/products/" + query.FirstOrDefault().ImageLink;
                }
            }

            //return default image place holder
            return "defaults/products/product-placeholder.jpg";
        }


        //get product image link using account id of uploader and image link passed.
        public static string GetProductImageLink(string account_id, string product_id, string image_link)
        {
            //if user directory name is (for some reason), return place holder image
            var directory_name = AccountHelper.GetProductImageDirectoty(account_id);
            if (directory_name == null)
            {
                return "defaults/products/product-placeholder.jpg";
            }

            using (var db = new DBConnection())
            {
                var query = db.ProductImages.Where(s => s.ProductID == product_id).OrderBy(s => s.ID).Take(1);
                if (query.Any())
                {
                    return directory_name + "/products/" + image_link;
                }
            }

            //return default image place holder
            return "defaults/products/product-placeholder.jpg";
        }

        //get product video link using account id of uploader and video link passed.
        public static string GetProductVideoLink(string account_id, string product_id, string image_link)
        {
            //if user directory name is (for some reason), return place holder video
            var directory_name = AccountHelper.GetProductImageDirectoty(account_id);
            if (directory_name == null)
            {
                return "defaults/products/product-placeholder.mp4";
            }

            using (var db = new DBConnection())
            {
                var query = db.ProductImages.Where(s => s.ProductID == product_id).OrderBy(s => s.ID).Take(1);
                if (query.Any())
                {
                    return directory_name + "/products/" + image_link;
                }
            }

            //return default image place holder
            return "defaults/products/product-placeholder.mp4";
        }

        //get navigation category list from database
        public static HtmlString GetNavCategoryList()
        {
            using (var db = new DBConnection())
            {
                var query = db.Categories.OrderBy(s => s.ID);
                if (query.Any())
                {
                    string result = "";
                    foreach (var item in query)
                    {
                        result += "<li><a href='/Collections/Category/" + item.CategoryName + "'>" + item.CategoryName + "</a></li>";
                    }
                    return new HtmlString(result);
                }
            }
            return new HtmlString("<li class='text-danger'><a href='#'>Not available</a></li>");
        }

        //get shop category list from database
        public static HtmlString GetShopCategoryList()
        {
            using (var db = new DBConnection())
            {
                var query = db.Categories.OrderBy(s => s.ID);
                if (query.Any())
                {
                    string result = "";
                    foreach (var item in query)
                    {
                        result += "<li class='mb-1'><a href='/Collections/Category/" + item.CategoryName + "' class='d-flex'> <span>" + item.CategoryName + "</span> <span class='text-black ml-auto'>("+GetProductCategoryCount(item.ID)+")</span> </a></li>";
                    }
                    return new HtmlString(result);
                }
            }
            return new HtmlString("<li class='mb-1 text-danger'><a href='#'>Not available</a></li>");
        }

        //get latest category list from database
        public static HtmlString GetLatestCategoryList()
        {
            using (var db = new DBConnection())
            {
                var query = db.Categories.OrderBy(s => s.ID);
                if (query.Any())
                {
                    string result = "";
                    foreach (var item in query)
                    {
                        result += "<a class='dropdown-item' href='/Shop/?l=" + item.CategoryName + "'>" + item.CategoryName + "</a>";
                    }
                    return new HtmlString(result);
                }
            }
            return new HtmlString("<a class='dropdown-item' href='#'>Not available</a>");
        }

        //get footer category list from database
        public static HtmlString GetFooterCategoryList()
        {
            using (var db = new DBConnection())
            {
                var query = db.Categories.OrderBy(s => s.ID);
                if (query.Any())
                {
                    string result = "";
                    foreach (var item in query)
                    {
                        result += "<li><a href='/Collections/Category/" + item.CategoryName + "'>" + item.CategoryName + "</a></li>";
                    }
                    return new HtmlString(result);
                }
            }
            return new HtmlString("<li><a href='#'>Not available</a></li>");
        }


        //get popular this week product id
        public static string GetWeeksPopularProduct()
        {
            using (var db = new DBConnection())
            {
                var query = db.PopularThisWeek;
                if (query.Any())
                {
                    return query.FirstOrDefault().ProductID;
                }
            }
            return null;
        }


        //get product currency symbol from currency code/product id
        public static string GetCurrencySymbol(string currency_code)
        {
            if (!string.IsNullOrEmpty(currency_code))
            {
                try
                {
                    using (var db = new DBConnection())
                    {
                        //if using product id insted of currency, get currency code from product id
                        if (currency_code.Length > 4)
                        {
                            currency_code = db.Products.Where(s => s.ProductID == currency_code).FirstOrDefault().Currency;
                        }

                        var query = db.Currency.Where(s => s.Code == currency_code);
                        if (query.Any())
                        {
                            return query.FirstOrDefault().Symbol;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO log error
                    Console.WriteLine(ex);
                }
            }
            return "NA";
        }


        //get specific product data. Takes product id and data to return
        public static string GetProductData(string product_id, string return_data)
        {
            try
            {
                if(!string.IsNullOrEmpty(product_id))
                {
                    using (var db = new DBConnection())
                    {
                        switch (return_data)
                        {
                            case "ProductName":
                                return db.Products.Where(s=> s.ProductID == product_id).FirstOrDefault().ProductName;
                            case "ProductDescription":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ProductDescription;
                            case "UniqueProductName":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().UniqueProductName;
                            case "ProductType":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ProductType.ToString();
                            case "WholeSaleQuantity":
                                if(db.Products.Where(s => s.ProductID == product_id).Any())
                                {
                                    return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().WholeSaleQuantity.ToString();
                                }
                                return null;
                            case "StoreID":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().StoreID.ToString();
                            case "CategoryID":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().CategoryID.ToString();
                            case "Currency":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().Currency;
                            case "ProductPrice":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ProductPrice;
                            case "ProductPreviousPrice":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ProductPreviousPrice;
                            case "ProductTags":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ProductTags;
                            case "ApproveStatus":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().ApproveStatus.ToString();
                            case "UpdatedBy":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().UpdatedBy;
                            case "UpdateDate":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().UpdateDate.ToString();
                            case "DateAdded":
                                return db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().DateAdded.ToString();
                            case "ImageLink":
                                var account_id = db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().AccountID;
                                return GetProductImageLink(account_id, product_id);
                            case "ProductCategory":
                                var category_id = db.Products.Where(s => s.ProductID == product_id).FirstOrDefault().CategoryID;
                                return db.Categories.Where(s => s.ID == category_id).FirstOrDefault().CategoryName;
                            default:
                                return "NA";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //TODO log error
                Console.WriteLine(ex);
            }
            return "NA";
        }

        //get product size name
        public static string GetSizeName(string size_acronym)
        {
            switch (size_acronym)
            {
                case "sm":
                    return "Small";
                case "md":
                    return "Medium";
                case "lg":
                    return "Large";
                case "xl":
                    return "Extra Large";
                default:
                    return "NA";
            }

        }
    }

}
