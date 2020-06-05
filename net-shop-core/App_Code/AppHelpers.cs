using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using ModestLiving.Models;
using System.Globalization;
using net_shop_core.Models;

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
            using (var db = new DBConnection())
            {
                var query = db.Accounts.Where(s => s.AccountID == account_id);
                if (query.Any())
                {
                    return query.FirstOrDefault().DirectoryName;
                }
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
            using (var db = new DBConnection())
            {
                var query = db.Categories.Where(s => s.ID == category_id);
                if (query.Any())
                {
                    return query.FirstOrDefault().CategoryName;
                }
            }
            return "Unknown";
        }

        //get product image link using account id of uploader. Takes the first image
        public static string GetProductImageLink(string account_id, int product_id)
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

        //get product image link using account id of uploader and image link passed. Takes the first image
        public static string GetProductImageLink(string account_id, int product_id, string image_link)
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
            return new HtmlString("<li><a href='#'>Not available</a></li>");
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
        public static int GetWeeksPopularProduct()
        {
            using (var db = new DBConnection())
            {
                var query = db.PopularThisWeek;
                if (query.Any())
                {
                    return query.FirstOrDefault().ProductID;
                }
            }
            return 0;
        }


        //get specific product data. Takes product id and data to return
        public static string GetProductData(int product_id, string return_data)
        {
            try
            {
                using (var db = new DBConnection())
                {
                    switch (return_data)
                    {
                        case "ProductName":
                            return db.Products.Where(s=> s.ID == product_id).FirstOrDefault().ProductName;
                        case "ProductDescription":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ProductDescription;
                        case "UniqueProductName":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().UniqueProductName;
                        case "ProductType":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ProductType.ToString();
                        case "WholeSaleQuantity":
                            if(db.Products.Where(s => s.ID == product_id).Any())
                            {
                                return db.Products.Where(s => s.ID == product_id).FirstOrDefault().WholeSaleQuantity.ToString();
                            }
                            return null;
                        case "StoreID":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().StoreID.ToString();
                        case "CategoryID":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().CategoryID.ToString();
                        case "ProductPrice":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ProductPrice;
                        case "ProductPreviousPrice":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ProductPreviousPrice;
                        case "ProductTags":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ProductTags;
                        case "ApproveStatus":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().ApproveStatus.ToString();
                        case "UpdatedBy":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().UpdatedBy;
                        case "UpdateDate":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().UpdateDate.ToString();
                        case "DateAdded":
                            return db.Products.Where(s => s.ID == product_id).FirstOrDefault().DateAdded.ToString();
                        case "ImageLink":
                            var account_id = db.Products.Where(s => s.ID == product_id).FirstOrDefault().AccountID;
                            return GetProductImageLink(account_id, product_id);
                        case "ProductCategory":
                            var category_id = db.Products.Where(s => s.ID == product_id).FirstOrDefault().CategoryID;
                            return db.Categories.Where(s => s.ID == category_id).FirstOrDefault().CategoryName;
                        default:
                            return "NA";
                    }
                }
            }
            catch(Exception ex)
            {
                //TODO log error
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
