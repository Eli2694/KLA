using Entity.EntityInterfaces;
using Model;

namespace Entity.Scanners
{
    public class EventScanner : IScanner
    {
        //public Dictionary<string, M_UniqueIds> RetriveFromDB(List<M_UniqueIds> ListFromDB)
        //{
           
        //}

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            List<M_UniqueIds> eventsDictionary = new List<M_UniqueIds>();

            foreach (var evnt in ktgemvar.Events)
            {
                eventsDictionary.Add( new M_UniqueIds { EntityType = "Event", ID = evnt.Id.ToString(), Name = evnt.Name, Scope = "event", Timestamp = DateTime.Now });
            }
            return eventsDictionary;
        }


        public bool compareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db)
        {
            return !db.Any(variableDB => XmlContainsVariable(xml, variableDB));
        }
        private bool XmlContainsVariable(List<M_UniqueIds> xml, M_UniqueIds variableDB)
        {
            return xml.Any(variableXML => variableDB.Name == variableXML.Name || variableDB.ID == variableXML.ID);
        }
    }
}