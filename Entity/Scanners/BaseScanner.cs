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

        public List<UniqueIds> newUniqueIdsFromXml;

        public BaseScanner(LogManager log)
        {
            newUniqueIdsFromXml = new List<UniqueIds>();
            _log = log;
        }

        

        public bool CompareXmlScopeWithDBScope(List<UniqueIds> xml, List<UniqueIds> db)
        {
            try
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
                        _log.LogError(errorMessage, LogProviderType.File);
                    }
                    return false;
                }

                AddUniqueIdsFromXmlToList(xml, dbByIdDictionary);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in CompareXmlScopeWithDBScope method: {ex.Message}", LogProviderType.File);
                return false;
            }
        }

        private void ValidateElements(List<UniqueIds> xml, Dictionary<string, UniqueIds> dbByIdDictionary, Dictionary<string, UniqueIds> dbByNameDictionary, List<string> errorMessages)
        {

            foreach (var xmlElement in xml)
            {
                ValidateNames(xmlElement, dbByIdDictionary, errorMessages);
                ValidateIds(xmlElement, dbByNameDictionary, errorMessages);
            }
        }

        private void ValidateNames(UniqueIds xmlElement, Dictionary<string, UniqueIds> dbByIdDictionary, List<string> errorMessages)
        {
            if (dbByIdDictionary.TryGetValue(xmlElement.ID, out var dbElementByID))
            {
                if (dbElementByID.Name != xmlElement.Name)
                {
                    errorMessages.Add($"ID '{xmlElement.ID}' has a different name in the XML and DB.");
                }
            }
        }

        private void ValidateIds(UniqueIds xmlElement, Dictionary<string, UniqueIds> dbByNameDictionary, List<string> errorMessages)
        {
            if (dbByNameDictionary.TryGetValue(xmlElement.Name, out var dbElementByName))
            {
                if (dbElementByName.ID != xmlElement.ID)
                {
                    errorMessages.Add($"Name '{xmlElement.Name}' has a different ID in the XML and DB.");
                }
            }
        }

        private void AddUniqueIdsFromXmlToList(List<UniqueIds> xml, Dictionary<string, UniqueIds> db)
        {
            newUniqueIdsFromXml = xml.Where(variableXML => !db.ContainsKey(variableXML.ID)).ToList();
            ReportNewUniqueIds();
        }

        private void ReportNewUniqueIds()
        {
            int count = 0;  // Counter for the total number of IDs

            _log.LogEvent($"Starting to log unique IDs from XML...", LogProviderType.Console);
            _log.LogEvent($"For detailed results, please see the generated log file.", LogProviderType.Console);

            foreach (var uniqueId in newUniqueIdsFromXml)
            {
                count++;

                // Reporting in a clear, easy-to-read format
                string message =
                    $"Unique ID Entry #{count}:\n" +
                    $"Entity Type: {uniqueId.EntityType}\n" +
                    $"ID: {uniqueId.ID}\n" +
                    $"Name: {uniqueId.Name}\n" +
                    $"\n";  // Adding a newline for clarity

                _log.LogEvent(message, LogProviderType.File);
            }

            _log.LogEvent($"Finished logging. Total number of unique IDs reported: {count}. Please refer to the text file for details.", LogProviderType.Console);
        }


    }

}
