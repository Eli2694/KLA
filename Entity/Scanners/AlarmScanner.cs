using Entity.EntityInterfaces;
using Model;
using Model.XmlModels;
using Utility_LOG;

namespace Entity.Scanners
{
    public class AlarmScanner : BaseScanner, IScanner 
    {
		public AlarmScanner(LogManager log) : base(log)
        {
        }
        public AlarmScanner() : base(null)
        {
        }

        public List<UniqueIds> ScanKtgemContent(Ktgem ktgemvar)
        {
            try
            {
                if (ktgemvar.Alarms != null)
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
                return new List<UniqueIds>();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
