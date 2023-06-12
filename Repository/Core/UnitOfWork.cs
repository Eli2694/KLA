using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KlaContext _context;

        public UnitOfWork(KlaContext context)
        {
            _context = context;
            UniqueIds = new UniqueIdsRepository(_context);
            Users = new UserRepository(_context);
        }


        public IUniqueIdsRepository UniqueIds { get; private set; }
        public IUserRepository Users { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
