using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Filters;
using Armut.Sample.Messaging.JWT;
using Armut.Sample.Messaging.Model;
using Armut.Sample.Messaging.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Armut.Sample.Messaging.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository m_UserRepository;
        private readonly ILogger<UserController> m_Logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            m_UserRepository = userRepository;
            m_Logger = logger;
        }

        [HttpPost("register")]
        [ValidationActionFilter]
        public IActionResult Register([Bind("UserName,EmailAddress,Password")]User user)
        {
            User result = m_UserRepository.Insert(user);
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.CreateResponse(ModelState));
            m_Logger.LogInformation("User Registered with Id: {0}", user.UserID);
            return Ok(ResponseFactory.CreateResponse(user));
        }

        [HttpGet("login")]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest();

            var user = m_UserRepository.GetByUserName(username);
            if (user == null)
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));
            else
            {
                if(PasswordHelper.Compare(user.Password, password))
                {
                    //TODO: move secret key to config
                    var token = new JwtTokenBuilder()
                        .AddSecurityKey(JwtSecurityKey.Create("armut.sample.messaging.secret"))
                        .AddSubject(user.UserName)
                        .AddIssuer("Armut.Sample.Messaging")
                        .AddAudience("Armut.Sample.Messaging")
                        .AddClaim("Username", user.UserName)
                        .AddExpiry(10)
                        .Build();
                    m_Logger.LogInformation("User Logged in with Id: {0}", user.UserID);
                    return Ok(token.Value);
                }
                else
                {
                    m_Logger.LogInformation("Invalid Log in with Id: {0}", user.UserID);
                    return Unauthorized();
                }

            }
        }



    }
}
