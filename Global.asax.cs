using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json;
using RojgarmitraSolution.Security;

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
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    PrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<PrincipalSerializeModel>(authTicket.UserData);
                    AdminPrincipal newUser = new AdminPrincipal(authTicket.Name);
                    newUser.UserID = serializeModel.UserID;
                    newUser.FullName = serializeModel.FullName;
                    newUser.LastName = serializeModel.LastName;
                    newUser.Roles = serializeModel.Roles;
                    newUser.AuthTocken = serializeModel.AuthTocken;
                    newUser.Email = serializeModel.Email;
                    newUser.Mobile = serializeModel.Mobile;
                    newUser.Role = serializeModel.Role;
                    newUser.Logo = serializeModel.Logo;                   
                    newUser.LoginCount = serializeModel.LoginCount;
                    HttpContext.Current.User = newUser;
                }
                catch (System.Security.Cryptography.CryptographicException cex)
                {
                    FormsAuthentication.SignOut();
                }
            }
        }
    }
}
