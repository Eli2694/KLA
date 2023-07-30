using DAL;
using Model;
using Repository.Interfaces;
using Utility_LOG;

namespace Repository.Core
{
    public class AliasesRepository : Repository<Aliases>, IAliasesRepository
    {
        public AliasesRepository(KlaContext context, LogManager log) : base(context, log)
        {
        }

    }
}
