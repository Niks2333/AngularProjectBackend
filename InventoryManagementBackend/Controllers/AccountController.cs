using System.Web.Http;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Models;

namespace InventoryManagementBackend.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly UserRepository repository = new UserRepository();


        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] UserModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Invalid login request");

            bool isValid = repository.ValidateUser(user.Username, user.Password);

            if (!isValid)
                return Unauthorized();


            string token = JwtManager.GenerateToken(user.Username);

            return Ok(new
            {
                token = token,
                Username = user.Username,
                Message = "Login successful"
            });
        }


    }
}