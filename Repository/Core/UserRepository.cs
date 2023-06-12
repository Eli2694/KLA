using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public class UserRepository : Repository<M_User>, IUserRepository
    {
        public UserRepository(KlaContext context) : base(context)
        {
        }

        public M_User GetValidatedUser(string userId)
        {
            return _context.Allowd_User.SingleOrDefault(n => n.UserID == userId);
        } 

    }
}
