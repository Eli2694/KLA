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
    }
}
