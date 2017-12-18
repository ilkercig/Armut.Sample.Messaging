using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Armut.Sample.Messaging.Controllers;
using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Model;
using Armut.Sample.Messaging.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit;
using NUnit.Framework;

namespace Armut.Sample.Messaging.Nunit.Test
{

    [TestFixture]
    public class MessageTest
    {
        private Mock<IMessageRepository> m_MessageRepoMock;
        private Mock<IUserRepository> m_UserRepoMock;
        private Mock<IBlockingRepository> m_BlockingRepository;
        private Mock<IDateTimeService> m_DateTimeServiceMock;
        private Mock<IModelFactory> m_ModelFactoryMock;
        private Mock<ILogger<MessageController>> m_LoggerMock;
        private User m_Receiver;
        private User m_Sender;

        [SetUp]
        public void Init()
        {
            m_MessageRepoMock = new Mock<IMessageRepository>();
            m_UserRepoMock = new Mock<IUserRepository>();
            m_BlockingRepository = new Mock<IBlockingRepository>();
            m_DateTimeServiceMock = new Mock<IDateTimeService>();
            m_ModelFactoryMock = new Mock<IModelFactory>();
            m_LoggerMock = new Mock<ILogger<MessageController>>();
        }

        [TearDown]
        public void Exit()
        {
            m_Receiver = null;
            m_Sender = null;
        }

        [Test]
        [TestCase("merhaba", "ilker")]
        [TestCase("adjkasdpjoğ/::€€€€@+++^^77/+++&&&&dpojpojpouduasoduao^^ÜÜÜÜsdu__???asdkpkl", "sombodyelse")]
        public void SendMessage_ToInvalidUser_ReturnNotFound(string body, string receiver)
        {
            
            m_Receiver = new User() { UserID = 1, UserName = receiver };
            m_Sender = new User() { UserID = 2, UserName = "rick" };
            User nullUser = null;
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Receiver.UserName)).Returns(nullUser);
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Sender.UserName)).Returns(m_Sender);

            var controller = CreateMessageControllerWithMocks();
            //Setup claim mocks
            SetUserClaim(controller, m_Sender.UserName);
            var result = controller.SendMessage(body, receiver);

            Assert.That(result, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.AreEqual("User Not Found", ((ResponseBase)((ObjectResult)result).Value).ErrorMessages[0]);

        }

        [Test]
        [TestCase("merhaba", "ilker")]
        [TestCase("adjkasdpjoğ/::€€€€@+++^^77/+++&&&&dpojpojpouduasoduao^^ÜÜÜÜsdu__???asdkpkl", "sombodyelse")]
        public void SendMessage_ModelState_IsInvalid(string body, string receiver)
        {
            //Dummy model objects
            DateTime dummyDate = DateTime.Now;
            m_Receiver = new User() { UserID = 1, UserName = receiver };
            m_Sender = new User() { UserID = 2, UserName = "rick" };
            Message message = new Message() { Body = null, SenderId = m_Sender.UserID, ReceiverId = m_Receiver.UserID, SentTime = dummyDate };

            //Setup Mocks
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Receiver.UserName)).Returns(m_Receiver);
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Sender.UserName)).Returns(m_Sender);
            m_DateTimeServiceMock.Setup(d => d.Now()).Returns(dummyDate);
            m_ModelFactoryMock.Setup(l => l.CreateMessage(body, m_Sender.UserID, m_Receiver.UserID, dummyDate)).Returns(message);


            var controller = CreateMessageControllerWithMocks();
            //Setup claim mocks
            SetUserClaim(controller, m_Sender.UserName);
            //Set dummy object validator
            SetObjectValidator(controller);
            //Add Fake validation errors
            controller.ModelState.AddModelError("SentTime", "Required");
            var result = controller.SendMessage(body, receiver);

            Assert.That(result, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.AreEqual("SentTime : Required", ((ResponseBase)((ObjectResult)result).Value).ErrorMessages[0]);
        }

        [Test]
        [TestCase("merhaba", "ilker")]
        [TestCase("adjkasdpjoğ/::€€€€@+++^^77/+++&&&&dpojpojpouduasoduao^^ÜÜÜÜsdu__???asdkpkl", "sombodyelse")]
        public void SendMessage_IsSuccesful(string body, string receiver)
        {
            //Dummy model objects
            DateTime dummyDate = DateTime.Now;
            m_Receiver = new User() { UserID = 1, UserName = receiver };
            m_Sender = new User() { UserID = 2, UserName = "rick" };
            Message message = new Message() { Body = body, SenderId = m_Sender.UserID, ReceiverId = m_Receiver.UserID, SentTime = dummyDate};
            
            //Setup Mock
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Receiver.UserName)).Returns(m_Receiver);
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Sender.UserName)).Returns(m_Sender);
            m_DateTimeServiceMock.Setup(d => d.Now()).Returns(dummyDate);
            m_ModelFactoryMock.Setup(l => l.CreateMessage(body, m_Sender.UserID, m_Receiver.UserID, dummyDate)).Returns(message);

            var controller = CreateMessageControllerWithMocks();
            //Set dummy object validator
            SetObjectValidator(controller);
            //Setup claim mocks
            SetUserClaim(controller, m_Sender.UserName);

            var result = controller.SendMessage(body, receiver);

            m_MessageRepoMock.Verify(m => m.Insert(message));
            Assert.That(result, Is.TypeOf(typeof(OkObjectResult)));
            Assert.AreEqual(body, ((Response<Message>)((ObjectResult)result).Value).Result.Body);
        }

        [Test]
        [TestCase("merhaba", "ilker")]
        [TestCase("adjkasdpjoğ/::€€€€@+++^^77/+++&&&&dpojpojpouduasoduao^^ÜÜÜÜsdu__???asdkpkl", "sombodyelse")]
        public void SendMessage_ToUserBlocksMe(string body, string receiver)
        {
            m_Receiver = new User() { UserID = 1, UserName = receiver };
            m_Sender = new User() { UserID = 2, UserName = "rick" };
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Sender.UserName)).Returns(m_Sender);
            m_UserRepoMock.Setup(l => l.GetByUserName(m_Receiver.UserName)).Returns(m_Receiver);
            //Set blocking
            m_BlockingRepository.Setup(l => l.GetUnwantsMe(m_Sender.UserID)).Returns(new List<User>() { m_Receiver});


            var controller = CreateMessageControllerWithMocks();
            //Setup claim mocks
            SetUserClaim(controller, m_Sender.UserName);
            var result = controller.SendMessage(body, receiver);

            Assert.That(result, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.AreEqual("User Not Found", ((ResponseBase)((ObjectResult)result).Value).ErrorMessages[0]);

        }

        [Test]
        [TestCase(null, "ilker")]
        [TestCase("hello", null)]
        [TestCase(null, null)]
        [TestCase("", "")]
        public void SendMessage_InvalidParameters_ReturnBadRequest(string body, string receiver)
        {
            var controller = CreateMessageControllerWithMocks();
            var result = controller.SendMessage(body, receiver);
            Assert.That(result, Is.TypeOf(typeof(BadRequestResult)));
        }





        [Test]
        [TestCase("ilker")]
        public void ViewUnread(string sender)
        {

        }


        #region Private Methods
        private void SetObjectValidator(ControllerBase controller)
        {
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            controller.ObjectValidator = objectValidator.Object;

        }

        private MessageController CreateMessageControllerWithMocks()
        {
            return new MessageController(m_MessageRepoMock.Object, m_UserRepoMock.Object, m_BlockingRepository.Object,
                m_ModelFactoryMock.Object, m_DateTimeServiceMock.Object, m_LoggerMock.Object);
        }

        private void SetUserClaim(ControllerBase controller, string username)
        {
            var claims = new List<Claim>(){ new Claim("Username", username)};
            var contextMock = new Mock<HttpContext>();
            var claimsrprincipal = new Mock<ClaimsPrincipal>();
            claimsrprincipal.Setup(c => c.Claims).Returns(claims);
            contextMock.Setup(x => x.User).Returns(claimsrprincipal.Object);

            controller.ControllerContext.HttpContext = contextMock.Object;
        }


        #endregion



    }
}
