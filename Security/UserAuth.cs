using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using BusinessModel;

namespace RojgarmitraSolution.Security
{
    public class UserAuth
    {
        public UserLoginInfo GetCurrenUser()
        {
            UserLoginInfo objUser = null;
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity;
                if (basicAuthenticationIdentity != null)
                {
                    IPrincipal threadPrincipal = System.Threading.Thread.CurrentPrincipal;

                    var infos = basicAuthenticationIdentity.Name;
                    string[] info = infos.Split('~');
                    objUser.UserID = Convert.ToInt32(info[0]);
                    objUser.Role = info[1];
                }
            }
            return objUser;
        }
    }
}
    