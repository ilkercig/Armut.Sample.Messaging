using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;

namespace Armut.Sample.Messaging.Repository
{
    public interface IMessageRepository : IRepository<Message>
    {

        /// <summary>
        /// Get unread or allMessage
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="allMessages">if this is false then only unread messages will return</param>
        /// <returns></returns>
        IEnumerable<Message> ViewAllOrUnreadMessages(int senderId, int receiverId, bool allMessages);

        void MarkAsRead(Message message, DateTime now);

    }
}