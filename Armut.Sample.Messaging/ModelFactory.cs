using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging
{
    public class ModelFactory : IModelFactory
    {
        public Blocking CreateBlocking(int blockerId, int unWantedId)
        {
            return new Blocking() { BlockerId = blockerId, UnWantedId = unWantedId };

        }

        public Message CreateMessage(string body, int senderId, int receiverId, DateTime sentTime)
        {
            return new Message()
            {
                Body = body,
                SenderId = senderId,
                ReceiverId = receiverId,
                SentTime = sentTime

            };
        }
    }
}
