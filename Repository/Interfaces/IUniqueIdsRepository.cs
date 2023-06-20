using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUniqueIdsRepository : IRepository<UniqueIds>
    {
        IEnumerable<UniqueIds> GetSpecificScope(string scope);
        IEnumerable<UniqueIds> GetUniqueIdsWithAliases();
    }
}
