using Entity.EntityInterfaces;
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
		public AlarmScanner(LogManager log ) : base(log)
        {
        }


		public List<UniqueIds> ScanCode(KlaXML ktgemvar)
        {
            try
            {
                return ktgemvar.Alarms.Select(alarm => new UniqueIds
                {
                    EntityType = "Alarm",
                    ID = alarm.Id.ToString(),
                    Name = alarm.Name,
                    Scope = "alarm",
                    Timestamp = DateTime.Now
                }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
