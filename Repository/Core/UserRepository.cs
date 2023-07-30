using DAL;
using Model;
using Repository.Interfaces;
using Utility_LOG;

namespace Repository.Core
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly LogManager _log;

        public UserRepository(KlaContext context, LogManager log) : base(context, log)
        {
            _log = log;
        }

        public User GetValidatedUser(string userId)
        {
            try
            {
                return _context.Users.SingleOrDefault(n => n.UserID == userId);
            }
            catch (Exception ex)
            {
                _log.LogException($"Error in method GetValidatedUser(): {ex.Message}", ex, LogProviderType.File);
                throw;
            }
            
        } 

    }
}
