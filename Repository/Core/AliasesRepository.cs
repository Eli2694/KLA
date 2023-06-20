﻿using DAL;
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
    public class AliasesRepository : Repository<Aliases>, IAliasesRepository
    {
        public AliasesRepository(KlaContext context, LogManager log) : base(context, log)
        {
        }

        /// <summary>
        /// this method is used to get all aliases associated with a unique id
        /// returns null if no aliases are found for the unique id
        /// 

        public UniqueIds GetUniqueIdWithAliases(string id, string scope)
        {
            try
            {


                var uniqueIdWithAliases = _context.Unique_Ids
                                                   .Include(u => u.Aliases)
                                                   .FirstOrDefault(u => u.ID == id && u.Scope == scope);
                return uniqueIdWithAliases;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
