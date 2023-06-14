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

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            List<M_UniqueIds> eventScopeList = new List<M_UniqueIds>();

            foreach (var evnt in ktgemvar.Events)
            {
                eventScopeList.Add(new M_UniqueIds { EntityType = "Event", ID = evnt.Id.ToString(), Name = evnt.Name, Scope = "event", Timestamp = DateTime.Now });
            }
            return eventScopeList;
        } 
    }

}