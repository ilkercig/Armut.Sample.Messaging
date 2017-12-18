using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Model;

namespace Armut.Sample.Messaging.Repository
{
    public class UserRepository : Repository<User> ,IUserRepository
    {
        public UserRepository(MessagingContext dbContext) : base(dbContext)
        {

        }

        public User GetByUserName(string username)
        {
            return m_DbContext.Users.SingleOrDefault(u => u.UserName == username);
        }

        public bool UserAlreadyExsist(User user)
        {
            return m_DbContext.Users.Any(u => (u.UserName == user.UserName) || (u.EmailAddress == user.EmailAddress));
        }
    }
}
