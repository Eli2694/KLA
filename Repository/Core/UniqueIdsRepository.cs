using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;
using Utility_LOG;

namespace Repository.Core
{
    public class UniqueIdsRepository : Repository<UniqueIds>, IUniqueIdsRepository
    {
        public UniqueIdsRepository(KlaContext context, LogManager log) : base(context, log)
        {
        }

        public IEnumerable<UniqueIds> GetSpecificScope(string scope)
        {
            try
            {
                return _context.Unique_Ids
                                .Where(x => x.Scope == scope)
                                .ToList();
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public IEnumerable<UniqueIds> GetUniqueIdsWithAliases()
        {
            try
            {
                return _context.Unique_Ids.Include(u => u.Aliases).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
