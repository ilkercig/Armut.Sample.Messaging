using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging
{
    public interface IModelFactory
    {
        Message CreateMessage(string body, int sender, int receiver, DateTime sentTime);

        Blocking CreateBlocking(int blockerId, int UnWantedId);
    }
}
