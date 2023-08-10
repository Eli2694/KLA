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
                foreach (var entry in ex.Entries)
                {
                    if (entry.State == EntityState.Modified)
                    {
                        var existingEntity = _context.Set<UniqueIds>() 
                            .FirstOrDefault(e => e.Scope == entry.OriginalValues["Scope"] && e.ID == entry.OriginalValues["ID"]);

                        if (existingEntity != null)
                        {
                            entry.State = EntityState.Detached;
                            _context.Attach(existingEntity);
                            _context.Entry(existingEntity).CurrentValues.SetValues(entry.CurrentValues);
                        }
                    }

                    entry.Reload();
                    _log.LogInfo($"Reloaded Entity: {entry}", LogProviderType.Console);
                }

                throw;
            }
            catch (DbUpdateException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.State == EntityState.Modified)
                    {
                        var existingEntity = _context.Set<UniqueIds>() 
                            .Local
                            .FirstOrDefault(e => e.Scope == entry.OriginalValues["Scope"] && e.ID == entry.OriginalValues["ID"]);

                        if (existingEntity != null)
                        {
                            entry.State = EntityState.Detached;
                            _context.Attach(existingEntity);
                            _context.Entry(existingEntity).CurrentValues.SetValues(entry.CurrentValues);
                        }
                    }

                    entry.Reload();
                    _log.LogInfo($"Reloaded Entity: {entry}", LogProviderType.Console);
                }

                throw;
            }

            catch (Exception ex)
            {
                _log.LogError($"An error occurred in Save: {ex.Message}", LogProviderType.File);
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
                _log.LogError($"An error occurred while disposing the database context: {ex.Message}", LogProviderType.File);
                throw;
            }
        }
    }
}
