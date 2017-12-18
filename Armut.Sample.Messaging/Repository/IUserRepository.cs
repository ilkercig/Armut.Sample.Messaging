using Armut.Sample.Messaging.Model;

namespace Armut.Sample.Messaging.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        bool UserAlreadyExsist(User user);
        User GetByUserName(string username);
    }
}