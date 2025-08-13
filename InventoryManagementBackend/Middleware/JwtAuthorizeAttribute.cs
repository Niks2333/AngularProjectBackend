using System.Web.Mvc;
using System;
using System.Security.Claims;
using System.Web;
using InventoryManagementLibrary.Helpers;

namespace InventoryMangement.Middleware
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private static readonly string SecretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            var authToken = request.Cookies["AuthToken"]?.Value;

            if (string.IsNullOrEmpty(authToken))
            {
                HandleUnauthorizedRequest(filterContext);

                return;
            }

            try
            {
                ClaimsPrincipal principal = JwtManager.GetPrincipal(authToken);

                HttpContext.Current.User = principal;
            }
            catch (Exception)
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}