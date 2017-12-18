using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Repository
{
    internal class BlockingRepository : Repository<Blocking>, IBlockingRepository
    {
        public BlockingRepository(MessagingContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<User> GetBlockedByMe(int userId)
        {
            var result =
                from b in m_DbContext.Blockings.Where(b=>b.BlockerId == userId)
                join u in m_DbContext.Users on b.UnWantedId equals u.UserID
                select new User(){ UserID = u.UserID, UserName = u.UserName, EmailAddress = u.EmailAddress };

            return result;
        }

        public IEnumerable<User> GetUnwantsMe(int userId)
        {
            var result =
                from b in m_DbContext.Blockings.Where(b => b.UnWantedId == userId)
                join u in m_DbContext.Users on b.BlockerId equals u.UserID
                select new User() { UserID = u.UserID, UserName = u.UserName, EmailAddress = u.EmailAddress };

            return result;
        }

        public bool IsBlockingExist(int blocker, int unWanted)
        {
            return m_DbContext.Blockings.Any(b => b.BlockerId == blocker && b.UnWantedId == unWanted);
        }
    }
}
