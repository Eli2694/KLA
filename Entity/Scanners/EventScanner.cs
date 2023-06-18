using Entity.EntityInterfaces;
using Model;
using Utility_LOG;

namespace Entity.Scanners
{
    public class EventScanner : BaseScanner, IScanner
    {
        
        public EventScanner(LogManager log) : base(log)
        {
        }

        public EventScanner() : base(null)
        {
        }

        public List<UniqueIds> ScanCode(KlaXML ktgemvar)
        {
            try
            {
                return ktgemvar.Events.Select(evnt => new UniqueIds
                {
                    EntityType = "Event",
                    ID = evnt.Id.ToString(),
                    Name = evnt.Name,
                    Scope = "event",
                    Timestamp = DateTime.Now
                })
                            .ToList();
            }
            catch (Exception)
            {

                throw;
            }     
        } 
    }
}