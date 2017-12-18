using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Data;
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

    public class MessageController : Controller
    {
        private readonly IMessageRepository m_messageRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IModelFactory m_ModelFactory;
        private readonly IDateTimeService m_DateTimseService;
        private readonly IBlockingRepository m_BlockingRepository;
        private readonly ILogger<MessageController> m_Logger;

        public User CurrentUser
        {
            get
            {
                var dict = new Dictionary<string, string>();
                this.HttpContext.User.Claims.ToList().ForEach(item => dict.Add(item.Type, item.Value));
                var user = m_UserRepository.GetByUserName(dict["Username"]);
                return user ?? throw new ArgumentException("username");
            }
        }

        public MessageController(IMessageRepository messageRepository, IUserRepository userRepository, IBlockingRepository blockingRepository,
            IModelFactory modelFactory, IDateTimeService dateTimeService, ILogger<MessageController> logger)
        {
            m_messageRepository = messageRepository;
            m_UserRepository = userRepository;
            m_ModelFactory = modelFactory;
            m_DateTimseService = dateTimeService;
            m_BlockingRepository = blockingRepository;
            m_Logger = logger;
        }

        [HttpPost("send")]
        public IActionResult SendMessage(string body, string receiver)
        {
            if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(receiver))
                return BadRequest();

            var receiverUser = m_UserRepository.GetByUserName(receiver);
            var unWantsMe = m_BlockingRepository.GetUnwantsMe(CurrentUser.UserID);

            //Blocked user can't send messages. I thought user who blocks someone should be invisible for them
            if (receiverUser == null || unWantsMe.Any(u=>u.UserID == receiverUser.UserID))
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));

            var message =  m_ModelFactory.CreateMessage(body, CurrentUser.UserID, receiverUser.UserID, m_DateTimseService.Now());

            if (TryValidateModel(message))
                m_messageRepository.Insert(message);
            else
                return BadRequest(ResponseFactory.CreateResponse(ModelState));

            m_Logger.LogInformation("Message sent to user {0} \n Message Body: {1} \n from user {0}", receiverUser.UserID, message.Body, CurrentUser.UserID);
            return Ok(ResponseFactory.CreateResponse(message));

        }

        [HttpGet("view")]
        public IActionResult ViewMessage(int messageId)
        {
            var message = m_messageRepository.GetById(messageId);

            if(message.ReceiverId == CurrentUser.UserID)
            {
                if (!message.Read)
                {
                    m_messageRepository.MarkAsRead(message, m_DateTimseService.Now());
                }
            }
            else if(message.SenderId != CurrentUser.UserID)
            {
                m_Logger.LogInformation("Unauthenticated attempt to view message : {0} from user {1}", message.MessageID, CurrentUser.UserID);
                return Unauthorized();
            }
            m_Logger.LogInformation("Message is viewed : {0} from user {1}", message.MessageID, CurrentUser.UserID);
            return Ok(message.Body);
        }

        [HttpGet("view/unread")]
        public IActionResult ViewUnreadMessagesBySender(string sender)
        {
            if (string.IsNullOrEmpty(sender))
                return BadRequest();

            var senderUser = m_UserRepository.GetByUserName(sender);

            if (senderUser == null)
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));

            var messages = m_messageRepository.ViewAllOrUnreadMessages(senderUser.UserID, CurrentUser.UserID, false);

            return Ok(ResponseFactory.CreateResponse(messages));
        }

        [HttpGet("view/all")]
        public IActionResult ViewAllMessagesBySender(string sender)
        {
            if (string.IsNullOrEmpty(sender))
                return BadRequest();

            var senderUser = m_UserRepository.GetByUserName(sender);

            if (senderUser == null)
                return NotFound(ResponseFactory.CreateResponse("User Not Found"));

            var messages = m_messageRepository.ViewAllOrUnreadMessages(senderUser.UserID, CurrentUser.UserID, true);

            return Ok(ResponseFactory.CreateResponse(messages));
        }


    }
}
