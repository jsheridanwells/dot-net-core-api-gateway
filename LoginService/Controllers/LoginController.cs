using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using LoginService.Models;
using LoginService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LoginService.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            User u = new UserRepository().GetUser(user.Name);

            if (u == null)
            {
                return NotFound();
            }

            bool creds = u.Password == user.Password;
            if (!creds)
            {
                return Unauthorized();
            }

            string token = TokenManager.GenerateToken(user.Name);
            
            return Ok(token);
        }
    }
}
