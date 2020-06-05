using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using net_shop_core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ModestLiving.Models
{
    public class AppFunctions
    {
        const string SessionViewedProducts = "_ViewedProducts";

        //Log product view to database
        public bool LogProductView(int product_id, string visitor_id, string visitor_ip, string visitor_browser, string visitor_device, string other)
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
        public bool IsProductViewed(int product_id, string _viewed_products)
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

    }

}
