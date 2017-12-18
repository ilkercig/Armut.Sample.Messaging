using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Model;

namespace Armut.Sample.Messaging.Repository
{
    internal class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(MessagingContext dbContext) : base(dbContext)
        {
        }



        public void MarkAsRead(Message message, DateTime now)
        {
            message.Read = true;
            message.ReceiveTime = now;
            base.Update(message);
        }

        public IEnumerable<Message> ViewAllOrUnreadMessages(int senderId, int receiverId, bool allMessages)
        {
            var messages =  m_DbContext.Messages.Where(l => (allMessages || !l.Read) && l.SenderId == senderId && l.ReceiverId == l.ReceiverId).OrderBy(l => l.SentTime);
            List<Message> returnValue = messages.Select(m => new Message() { Body = m.Body, ReceiverId = m.ReceiverId, SenderId = m.SenderId, SentTime = m.SentTime }).ToList();

            foreach (Message message in messages)
            {
                message.Read = true;
                message.ReceiveTime = DateTime.Now;
            }

            m_DbContext.SaveChanges();
            return returnValue;
        }


    }
}
