using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Web.WebPages.OAuth;


namespace RojgarmitraSolution
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public class OAuthConfig
        {
            public static void RegisterProvides()
            {
                OAuthWebSecurity.RegisterGoogleClient();
               
            }
        }
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OAuthConfig.RegisterProvides();
        }
    }
}
