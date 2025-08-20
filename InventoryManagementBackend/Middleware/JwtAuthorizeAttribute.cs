using System;
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
                HttpContext.Current.User = principal;
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
