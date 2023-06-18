using DAL;
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
    public class AliasesRepository : Repository<Aliases>, IAliasesRepository
    {
        public AliasesRepository(KlaContext context, LogManager log) : base(context, log)
        {
        }
    }
}
