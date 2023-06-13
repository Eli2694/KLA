using Entity.EntityInterfaces;
using Model;

namespace Entity.Scanners
{
    public class EventScanner : IScanner
    {
        //public Dictionary<string, M_UniqueIds> RetriveFromDB(List<M_UniqueIds> ListFromDB)
        //{
           
        //}

        public Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            Dictionary<string, M_UniqueIds> eventsDictionary = new Dictionary<string, M_UniqueIds>();

            foreach (var evnt in ktgemvar.Events)
            {
                string ID_KEY = evnt.Id.ToString();
                eventsDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "Event", ID = ID_KEY, Name = evnt.Name, Scope = "Events", Timestamp = DateTime.Now });
            }
            return eventsDictionary;
        }


        public bool compareXmlScopeWithDBScope(Dictionary<string, M_UniqueIds> xml, Dictionary<string, M_UniqueIds> db)
        {
            return !db.Values.Any(variableDB => XmlContainsVariable(xml, variableDB));

        }
        private bool XmlContainsVariable(Dictionary<string, M_UniqueIds> xml, M_UniqueIds variableDB)
        {
            return xml.Values.Any(variableXML => variableDB.Name == variableXML.Name || variableDB.ID == variableXML.ID);
        }
    }
}