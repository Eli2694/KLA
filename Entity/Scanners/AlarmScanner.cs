﻿using Entity.EntityInterfaces;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.Scanners
{
    public class AlarmScanner : BaseScanner, IScanner 
    {
        
        public AlarmScanner(LogManager log) : base(log)
        {
        }

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            return ktgemvar.Alarms.Select(alarm => new M_UniqueIds
            {
                EntityType = "Alarm",
                ID = alarm.Id.ToString(),
                Name = alarm.Name,
                Scope = "alarm",
                Timestamp = DateTime.Now
            }).ToList();
        }
    }
}
