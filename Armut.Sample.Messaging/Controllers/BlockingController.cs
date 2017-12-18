using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Filters;
using Armut.Sample.Messaging.Model;
using Armut.Sample.Messaging.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Armut.Sample.Messaging.Controllers
{
    [Authorize(Policy = "User")]
    [Route("api/[controller]")]
    public class BlockingController : Controller
    {
        private readonly IBlockingRepository m_BlockingRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IModelFactory m_ModelFactory;

        public User CurrentUser
        {
            get
            {
                var dict = new Dictionary<string, string>();
                HttpContext.User.Claims.ToList().ForEach(item => dict.Add(item.Type, item.Value));
                var user = m_UserRepository.GetByUserName(dict["Username"]);
                return user ?? throw new ArgumentException("username");   
            }
        }

        public BlockingController(IBlockingRepository blockingRepository, IUserRepository userRepository, IModelFactory modelFactory, ILogger<BlockingController> logger)
        {
            m_BlockingRepository = blockingRepository;
            m_UserRepository = userRepository;
            m_ModelFactory = modelFactory;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            string[] arrRetValues = null;
            if (arrRetValues.Length > 0)
            { }
            var result = m_BlockingRepository.GetBlockedByMe(CurrentUser.UserID);
            return Ok(result);
        }

        // PUT api/<controller>/5
        [HttpPut]
        public IActionResult Put(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest();
            
            var userBlocked = m_UserRepository.GetByUserName(username);

            if (userBlocked == null)
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));

            var blocking = m_ModelFactory.CreateBlocking(CurrentUser.UserID, userBlocked.UserID);
            if(TryValidateModel(blocking))
                m_BlockingRepository.Insert(blocking);
            else
                return BadRequest(ResponseFactory.CreateResponse(ModelState));

            return Ok();
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public IActionResult Delete(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest();

            var userBlocked = m_UserRepository.GetByUserName(username);

            if (userBlocked == null)
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));

            if (!m_BlockingRepository.IsBlockingExist(CurrentUser.UserID, userBlocked.UserID))
                return NotFound(ResponseFactory.CreateResponse("Blocking Not Found"));

            var blocking = m_ModelFactory.CreateBlocking(CurrentUser.UserID, userBlocked.UserID);
            m_BlockingRepository.Delete(blocking);

            return Ok();
        }
    }
}
