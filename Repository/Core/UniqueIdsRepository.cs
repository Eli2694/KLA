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
    public class UniqueIdsRepository : Repository<UniqueIds>, IUniqueIdsRepository
    {
        public UniqueIdsRepository(KlaContext context) : base(context)
        {
        }

        public IEnumerable<UniqueIds> GetSpecificScope(string scope)
        {
            return _context.Unique_Ids
                .Where(x => x.Scope == scope)
                .ToList();
        }

    }
}
