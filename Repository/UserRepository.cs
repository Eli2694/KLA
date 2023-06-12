using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IRepositoryUsers
    {
        private readonly DbAccess _dbAccess;

        public UserRepository(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }
        public void AddUser(M_User user)
        {
            try
            {
                _dbAccess.Add(user);
                _dbAccess.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User {user.UserID} Already Exists");
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
