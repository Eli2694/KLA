using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUserRepository : IRepository<M_User>
    {
        M_User GetValidatedUser(string userId);
    }
}
