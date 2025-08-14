using System;
using System.Web;
using System.Web.Http;
using InventoryManagementLibrary.Models;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.DAL;

namespace InventoryManagementBackend.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UserRepository repository = new UserRepository();

     
        [HttpPost]
        [Route("api/account/login")]
        public IHttpActionResult Login([FromBody] UserModel user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                    return Ok(new { success = false, message = "Username and password required." });

                string userEmail = repository.ValidateUserAndGetEmail(user.Username, user.Password);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    
                    HttpContext.Current.Session["LoginUsername"] = user.Username;
                    HttpContext.Current.Session["LoginUserEmail"] = userEmail;

                    int otp = EmailHelper.GenerateOtp();
                    HttpContext.Current.Session["UserOTP"] = otp;

                  
                    string subject = "Your OTP - Inventory Login";
                    string body = $"Hello, here is your OTP: <b>{otp}</b> Valid for 10 minutes.";
                    EmailHelper.SendEmail(userEmail, subject, body);

                    return Ok(new { success = true, email = userEmail });
                }

                return Ok(new { success = false, message = "Invalid credentials." });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return Ok(new { success = false, message = "An error occurred during login." });
            }
        }

      
        [HttpPost]
        [Route("api/account/verify-otp")]
        public IHttpActionResult VerifyOTP([FromBody] OtpVerifyModel model)
        {
            try
            {
                if (HttpContext.Current.Session["UserOTP"] != null &&
                    model.Otp == HttpContext.Current.Session["UserOTP"].ToString())
                {
                    string username = HttpContext.Current.Session["LoginUsername"].ToString();
                    string token = JwtManager.GenerateToken(username);

                   
                    var cookie = new HttpCookie("AuthToken", token)
                    {
                        HttpOnly = true,
                        Secure = HttpContext.Current.Request.IsSecureConnection,
                        Expires = DateTime.UtcNow.AddMinutes(60)
                    };
                    HttpContext.Current.Response.Cookies.Add(cookie);

                    HttpContext.Current.Session.Remove("UserOTP");

                    return Ok(new { success = true, token });
                }

                return Ok(new { success = false, message = "Incorrect OTP." });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return Ok(new { success = false, message = "An error occurred during OTP verification." });
            }
        }

     
        [HttpPost]
        [Route("api/account/logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["AuthToken"] != null)
                {
                    var cookie = new HttpCookie("AuthToken")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true
                    };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }

                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();

                return Ok(new { success = true, message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return Ok(new { success = false, message = "An error occurred during logout." });
            }
        }
    }







}
