using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
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
            catch (DbUpdateConcurrencyException ex)
            {
                HandleDbUpdateException(ex, "Reloaded Entity");
            }
            catch (DbUpdateException ex)
            {
                HandleDbUpdateException(ex, "Reloaded Entity");
            }
            catch (Exception ex)
            {
                HandleGeneralException(ex, "An error occurred in Save");
            }
        }

        private void HandleDbUpdateException(DbUpdateException ex, string action)
        {
            foreach (var entry in ex.Entries)
            {
                entry.Reload();
            }
            throw ex;
        }

        private void HandleGeneralException(Exception ex, string message)
        {
            _log.LogError($"{message}: {ex.Message}", LogProviderType.File);
            throw ex;
        }

        public void Dispose()
        {
            try
            {
                _context.Dispose();
            }
            catch (Exception ex)
            {
                HandleGeneralException(ex, "An error occurred while disposing the database context");
            }
        }
    }
}
