using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModestLiving.Models
{
    //Session manager class
    /// <summary>
    /// To access the Session in Startup.ConfigureServices
    /// </summary>
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _sessionContext => _httpContextAccessor.HttpContext.Session;
        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Sets the session for viewed products
        /// </summary>
        /// <returns>void</returns>
        public void SetViewedProducts(string product_id)
        {
            _sessionContext.SetString("ViewedProducts", GetViewedProducts() + ",product_id_" + product_id);
        }


        /// <summary>
        /// Gets the current viewed products
        /// </summary>
        /// <returns>viewed products as comma separated strings</returns>
        public string GetViewedProducts()
        {
            if (string.IsNullOrEmpty(_sessionContext.GetString("ViewedProducts")))
            {
                SetViewedProducts("");
            }
            return _sessionContext.GetString("ViewedProducts").TrimStart(',');
        }


        public void TestSet(string session)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
               
            }
            else
            {
                /// Then I’m doing something with the request context
            }
            _sessionContext.SetString("Test", session);
        }

        public string TestGet()
        {
            if (string.IsNullOrEmpty(_sessionContext.GetString("Test")))
            {
                TestSet("");
            }
            return _sessionContext.GetString("Test");
        }


    }
}
