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

        public void Complete()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle the concurrency exception.
                // You could log the error message and/or display a user-friendly message to the end user.
                Console.WriteLine($"Concurrency error in Save: {ex.Message}");

                // If necessary, you can rethrow the exception to propagate the error upwards.
                throw;
            }
            catch (DbUpdateException ex)
            {
                // This is another type of exception that can be thrown by SaveChanges.
                // It can be due to SQL Server errors, connectivity issues, etc.
                Console.WriteLine($"Database update error in Save: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // This will catch any other types of exceptions.
                Console.WriteLine($"An error occurred in Save: {ex.Message}");
                throw;
            }

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
