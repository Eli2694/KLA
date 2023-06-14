using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.Scanners
{
    public  class BaseScanner
    {
        protected readonly LogManager _log;
        private List<M_UniqueIds> newUniqueIdsFromXml;

        public BaseScanner(LogManager log)
        {
            _log = log;
            newUniqueIdsFromXml = new List<M_UniqueIds>();
        }
        public bool CompareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db)
        {
            var dbDictionary = db.ToDictionary(k => k.ID, v => v);
            var dbByNameDictionary = db.ToDictionary(k => k.Name, v => v);
            bool isValid = true;
            foreach (var xmlElement in xml)
            {
                bool hasDifferentName =  HasDifferentNameAsync(xmlElement, dbDictionary);
                bool hasDifferentID =  HasDifferentIDAsync(xmlElement, dbByNameDictionary);

                if (hasDifferentName && hasDifferentID)
                {
                    this.newUniqueIdsFromXml.Add(xmlElement);
                }
                else
                {
                    isValid = false;
                }

            }
            return isValid;
        }

        private bool HasDifferentNameAsync(M_UniqueIds xmlElement, Dictionary<string, M_UniqueIds> db)
        {
                if (db.TryGetValue(xmlElement.ID, out var dbElementByID))
                {
                    if (dbElementByID.Name != xmlElement.Name)
                    {
                        _log.LogError($"ID '{xmlElement.ID}' has a different name in the XML and DB.", LogProviderType.Console);
                    }
                    return false;
                }
                return true;         
        }

        private bool HasDifferentIDAsync(M_UniqueIds xmlElement, Dictionary<string, M_UniqueIds> db)
        {
            if (db.TryGetValue(xmlElement.Name, out var dbElementByName))
            {
                if (dbElementByName.ID != xmlElement.ID)
                {
                     _log.LogError($"Name '{xmlElement.Name}' has a different ID in the XML and DB.", LogProviderType.Console);
                }
              return false;
            }
            return true;
        }
        public void ReportNewUniqueIds()
        {
            Console.WriteLine("Unique IDs present in XML but not in DB:");
            foreach (var uniqueId in newUniqueIdsFromXml)
            {
                Console.WriteLine($"Entity Type: {uniqueId.EntityType}, ID: {uniqueId.ID}, Name: {uniqueId.Name}, Scope: {uniqueId.Scope}, Timestamp: {uniqueId.Timestamp}");
            }
        }
    }
}
