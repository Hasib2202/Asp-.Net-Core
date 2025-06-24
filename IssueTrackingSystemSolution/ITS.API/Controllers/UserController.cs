using ITS.BLL.DTOs;
using ITS.BLL.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITS.API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        // POST api/user/register
        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(UserDTO userDto)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var result = UserService.Register(userDto);
            if (result)
                return Request.CreateResponse(HttpStatusCode.Created, "User registered successfully.");
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username or email already exists.");
        }

        // POST api/user/login
        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var user = UserService.Login(loginDto.Username, loginDto.Password, out string token);
            if (user == null)
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid username or password.");

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                message = user.Role == "Admin" ? "Welcome Admin" : "Welcome User",
                user,
                token
            });
        }   


        // POST api/user/logout
        [HttpPost]
        [Route("logout")]
        public HttpResponseMessage Logout()
        {
            var authHeader = Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme != "Bearer")
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Authorization header missing or invalid.");

            var token = authHeader.Parameter;
            var success = UserService.Logout(token);
            if (success)
                return Request.CreateResponse(HttpStatusCode.OK, "User logged out successfully.");
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid token or already logged out.");
        }
    }
}
