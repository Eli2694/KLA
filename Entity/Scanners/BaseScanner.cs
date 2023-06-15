using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.Scanners
{
    public abstract class BaseScanner
    {
        protected readonly LogManager _log;
        public List<M_UniqueIds> newUniqueIdsFromXml;
        public BaseScanner(LogManager log)
        {
            newUniqueIdsFromXml = new List<M_UniqueIds>();
            _log = log;
        }

        public bool CompareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db)
        {
            var dbByIdDictionary = db.ToDictionary(k => k.ID, v => v);
            var dbByNameDictionary = db.ToDictionary(k => k.Name, v => v);
            var errorMessages = new List<string>();

            ValidateElements(xml, dbByIdDictionary, dbByNameDictionary, errorMessages);

            if (errorMessages.Count > 0)
            {
                foreach (var errorMessage in errorMessages)
                {
                    _log.LogError(errorMessage, LogProviderType.Console);
                }
                return false;
            }

            AddUniqueIdsFromXmlToList(xml, dbByIdDictionary);
            return true;
        }

        private void ValidateElements(List<M_UniqueIds> xml, Dictionary<string, M_UniqueIds> dbByIdDictionary, Dictionary<string, M_UniqueIds> dbByNameDictionary, List<string> errorMessages)
        {
            foreach (var xmlElement in xml)
            {
                ValidateNames(xmlElement, dbByIdDictionary, errorMessages);
                ValidateIds(xmlElement, dbByNameDictionary, errorMessages);
            }
        }

        private void ValidateNames(M_UniqueIds xmlElement, Dictionary<string, M_UniqueIds> dbByIdDictionary, List<string> errorMessages)
        {
            if (dbByIdDictionary.TryGetValue(xmlElement.ID, out var dbElementByID))
            {
                if (dbElementByID.Name != xmlElement.Name)
                {
                    errorMessages.Add($"ID '{xmlElement.ID}' has a different name in the XML and DB.");
                }
            }
        }

        private void ValidateIds(M_UniqueIds xmlElement, Dictionary<string, M_UniqueIds> dbByNameDictionary, List<string> errorMessages)
        {
            if (dbByNameDictionary.TryGetValue(xmlElement.Name, out var dbElementByName))
            {
                if (dbElementByName.ID != xmlElement.ID)
                {
                    errorMessages.Add($"Name '{xmlElement.Name}' has a different ID in the XML and DB.");
                }
            }
        }

        private void AddUniqueIdsFromXmlToList(List<M_UniqueIds> xml, Dictionary<string, M_UniqueIds> db)
        {
            newUniqueIdsFromXml = xml.Where(variableXML => !db.ContainsKey(variableXML.ID)).ToList();
            ReportNewUniqueIds();
        }

        private void ReportNewUniqueIds()
        {
            foreach (var uniqueId in newUniqueIdsFromXml)
            {
                _log.LogEvent($"Entity Type: {uniqueId.EntityType}, ID: {uniqueId.ID}, Name: {uniqueId.Name}", LogProviderType.Console);
            }
        }
    }

}
