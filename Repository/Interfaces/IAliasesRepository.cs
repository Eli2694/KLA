﻿using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAliasesRepository : IRepository<Aliases>
    {
        public UniqueIds GetUniqueIdWithAliases(string id, string scope);
    }
}
