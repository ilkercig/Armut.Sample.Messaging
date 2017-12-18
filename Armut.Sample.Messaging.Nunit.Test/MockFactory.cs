using System;
using System.Collections.Generic;
using System.Text;
using Armut.Sample.Messaging.Repository;
using Moq;

namespace Armut.Sample.Messaging.Nunit.Test
{
    internal static class MockFactory
    {
        internal static Mock<IMessageRepository> CreateMessageRepositoryMock()
        {
            return new Mock<IMessageRepository>();
            
        }
    }
}
