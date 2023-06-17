using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Repository.Core
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(KlaContext context, LogManager log) : base(context, log)
        {
        }

        public User GetValidatedUser(string userId)
        {
            try
            {
                return _context.Users.SingleOrDefault(n => n.UserID == userId);
            }
            catch (Exception)
            {

                throw;
            }
            
        } 

    }
}
