﻿using Entity.EntityInterfaces;
using Model;

namespace Entity.Scanners
{
    public class EventScanner : IScanner
    {
        public Dictionary<string, M_UniqueIds> RetriveFromDB()
        {
            throw new NotImplementedException();
        }

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
    }
}