using Model;

namespace Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetValidatedUser(string userId);
    }
}
