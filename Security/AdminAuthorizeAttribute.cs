using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessModel;
using RojgarmitraSolution.Security;
namespace RojgarmitraSolution.Security
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual AdminPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as AdminPrincipal; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(Roles))
                {
                    if (!CurrentUser.IsInRole(Roles))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Unauthorized", area = "" }));
                        //base.OnAuthorization(filterContext); //returns to login url
                    }
                }

                //---------------Check url Privilege-----------------------
                //checkPrivilege(filterContext);
                

                if (!String.IsNullOrEmpty(Users))
                {
                    if (!Users.Contains(CurrentUser.UserID.ToString()))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Index", area = "" }));
                        // base.OnAuthorization(filterContext); //returns to login url
                    }
                }
            }
            else
            {
                string returnUrl = null;
                if (filterContext.HttpContext.Request.HttpMethod.Equals("GET", System.StringComparison.CurrentCultureIgnoreCase))
                    returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Index", area = "" }));
                // base.OnAuthorization(filterContext); //returns to login url
            }
        }

        //private void checkPrivilege(AuthorizationContext filterContext)
        //{
        //    var rd = filterContext.RouteData;
        //    string currentAction = rd.GetRequiredString("action");
        //    string currentController = rd.GetRequiredString("controller");
        //    string currentArea = "";
        //    if (filterContext.RequestContext.RouteData.DataTokens.ContainsKey("area"))
        //    {
        //        currentArea = filterContext.RequestContext.RouteData.DataTokens["area"] as string;
        //    }

        //    var UrlPrivilegeList = new List<BussinessEntities.UrlPrivilegeEntity>();
        //    if (HttpContext.Current.Session["UserPrivilege"] != null)
        //    {
        //        UrlPrivilegeList = HttpContext.Current.Session["UserPrivilege"] as List<BussinessEntities.UrlPrivilegeEntity>;
        //    }
        //    else
        //    {
        //        var loginServices = new BussinessServices.LoginServices();
        //        UrlPrivilegeList = loginServices.GetUrlPrivilegeByUserID(CurrentUser.UserID);
        //        HttpContext.Current.Session["UserPrivilege"] = UrlPrivilegeList;
        //    }
        //    if (UrlPrivilegeList.Count > 0)
        //    {
        //        var available = UrlPrivilegeList.Where(x => string.Compare(x.Area, currentArea, true) == 0 && string.Compare(x.Controller, currentController, true) == 0 && string.Compare(x.Action, currentAction, true) == 0).Select(x => x.IsHasPrivilege);
        //        if (available.Any() && !available.Contains(true))
        //        {
        //            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Unauthorized", area = "" }));
        //        }
        //    }
        //}
    }

}