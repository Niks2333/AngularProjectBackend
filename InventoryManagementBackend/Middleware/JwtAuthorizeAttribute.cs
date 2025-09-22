using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using InventoryManagementLibrary.Helpers;

namespace InventoryMangement.Middleware
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var authHeader = request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer")
                return false;

            var token = authHeader.Parameter;

            try
            {
                ClaimsPrincipal principal = JwtManager.GetPrincipal(token);

                if (principal == null || !principal.Identity.IsAuthenticated)
                    return false;

                HttpContext.Current.User = principal;

          
                if (!string.IsNullOrEmpty(Roles))
                {
                    var allowedRoles = Roles.Split(',').Select(r => r.Trim());

                   
                    return allowedRoles.Any(role => principal.IsInRole(role));
                }

                return true; 
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request
                .CreateResponse(HttpStatusCode.Unauthorized,
                    new { message = "Unauthorized - Invalid or missing token" });
        }
    }
}
