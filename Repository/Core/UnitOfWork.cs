using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Utility_LOG;

namespace Repository.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KlaContext _context;
        private readonly LogManager _log;

        public UnitOfWork(KlaContext context, LogManager log)
        {
            _context = context;
            _log = log;
            UniqueIds = new UniqueIdsRepository(_context, log);
            Users = new UserRepository(_context, log);
            Aliases = new AliasesRepository(_context, log);

        }

        public IUniqueIdsRepository UniqueIds { get; private set; }
        public IUserRepository Users { get; private set; }

        public IAliasesRepository Aliases { get; private set; }

        public void Complete()
        {
            try
            {
                _log.LogEvent("Save changes to database", LogProviderType.File);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError($"{ex.Message}", LogProviderType.File);
                throw;
            }
        }
        public void Dispose()
        {
            try
            {
                _context.Dispose();
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred while disposing the database context, {ex.Message}", LogProviderType.File);
                throw;
            }
        }
    }
}
