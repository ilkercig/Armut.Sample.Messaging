using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Repository
{
    public interface IBlockingRepository : IRepository<Blocking>
    {
        IEnumerable<User> GetBlockedByMe(int userId);
        IEnumerable<User> GetUnwantsMe(int userId);

        bool IsBlockingExist(int blocker, int unWanted);
    }
}
