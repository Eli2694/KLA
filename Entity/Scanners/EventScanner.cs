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


        public bool compareFileWithDB(Dictionary<string, M_UniqueIds> xml, Dictionary<string, M_UniqueIds> db)
        {
            foreach (var eventDB in db)
            {
                foreach (var eventXML in xml)
                {
                    if (eventDB.Value.Name == eventXML.Value.Name || eventDB.Value.ID == eventXML.Value.ID)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}