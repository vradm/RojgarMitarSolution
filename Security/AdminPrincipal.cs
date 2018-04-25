using BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RojgarmitraSolution.Security
{
    public class AdminPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get; private set;
        }

        public bool IsInRole(string role)
        {
            if (role.Contains(','))
            {
                foreach (var item in role.Split(','))
                {
                    if (Roles.Any(x => x.Contains(item)))
                    {
                        return true;
                    }
                }
            }
            return Roles.Any(x => x.Contains(role));
        }

        
        public AdminPrincipal(string Username)
        {
            this.Identity = new GenericIdentity(Username);
        }
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Roles { get; set; }
        public string Role { get; set; }
        public string AuthTocken { get; set; }     
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Logo { get; set; }
        public string FullName { get; set; }       
        public int LoginCount { get; set; }
    }
    public class PrincipalSerializeModel
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        //public string LastName { get; set; }
        public string[] Roles { get; set; }
        public string Role { get; set; }
        public string AuthTocken { get; set; }        
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Logo { get; set; }      
        public int LoginCount { get; set; } = 0;
    }
    
}