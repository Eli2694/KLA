using Model;
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


        public bool CompareXmlScopeWithDBScope(List<UniqueIds> xml, List<UniqueIds> db, bool getFullInfo)
        {
            try
            {

                // Create a dictionary where the keys are ID values and the values are the corresponding items from the 'db' collection
                var dbByIdDictionary = db.ToDictionary(k => k.ID, v => v);

                // Create another dictionary where the keys are Name values and the values are the corresponding items from the 'db' collection
                var dbByNameDictionary = db.ToDictionary(k => k.Name, v => v);

                var errorMessages = new List<string>();

                // Validate the elements in the XML against the DB
                ValidateElements(xml, dbByIdDictionary, dbByNameDictionary, errorMessages);

                // Check if there are any validation errors
                if (errorMessages.Count > 0)
                {
                    foreach (var errorMessage in errorMessages)
                    {
                        _log.LogError(errorMessage, LogProviderType.Console);
                        _log.LogError(errorMessage, LogProviderType.File);
                    }
                    return false;
                }

                // If validation passed, add unique IDs from XML to the DB
                AddUniqueIdsFromXmlToList(xml, dbByIdDictionary, getFullInfo);

                // Return true indicating successful validation and addition
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
                // Validate the names of elements in the XML against the DB
                ValidateNames(xmlElement, dbByIdDictionary, errorMessages);

                // Validate the IDs of elements in the XML against the DB
                ValidateIds(xmlElement, dbByNameDictionary, errorMessages);
            }
        }

        private void ValidateNames(UniqueIds xmlElement, Dictionary<string, UniqueIds> dbByIdDictionary, List<string> errorMessages)
        {
            // Check if the ID from the XML exists in the DB
            if (dbByIdDictionary.TryGetValue(xmlElement.ID, out var dbElementByID))
            {
                // Compare the name from the XML with the corresponding name in the DB
                if (dbElementByID.Name != xmlElement.Name)
                {
                    errorMessages.Add($"ID '{xmlElement.ID}' has a different name in the XML and DB.");
                }
            }
        }

        private void ValidateIds(UniqueIds xmlElement, Dictionary<string, UniqueIds> dbByNameDictionary, List<string> errorMessages)
        {
            // Check if the Name from the XML exists in the DB
            if (dbByNameDictionary.TryGetValue(xmlElement.Name, out var dbElementByName))
            {
                // Compare the ID from the XML with the corresponding ID in the DB
                if (dbElementByName.ID != xmlElement.ID)
                {
                    errorMessages.Add($"Name '{xmlElement.Name}' has a different ID in the XML and DB.");
                }
            }
        }

        private void AddUniqueIdsFromXmlToList(List<UniqueIds> xml, Dictionary<string, UniqueIds> db, bool getFullInfo)
        {
            if (xml.Count > 0)
            {
                // Find new unique IDs from the XML that are not present in the 'db' dictionary
                newUniqueIdsFromXml = xml.Where(variableXML => !db.ContainsKey(variableXML.ID)).ToList();

                // Take the first item from the 'xml' list (assuming it's representative) and report new unique IDs
                UniqueIds item = xml[0];
                ReportNewUniqueIds(item.Scope, getFullInfo);
            }
        }

        private void ReportNewUniqueIds(string scope, bool getFullInfo)
        {
            int count = 0;  // Counter for the total number of IDs

            _log.LogInfo($"Initializing verification process of Unique IDs From {scope} in XML...\n", LogProviderType.Console);

            // Iterate through the newUniqueIdsFromXml list and report the unique IDs
            foreach (var uniqueId in newUniqueIdsFromXml)
            {
                count++;

                if (getFullInfo)
                {
                    // Report detailed information about the unique ID if getFullInfo is true
                    string message =
                        $"Unique ID Entry #{count}:\n" +
                        $"Entity Type: {uniqueId.EntityType}\n" +
                        $"ID: {uniqueId.ID}\n" +
                        $"Name: {uniqueId.Name}\n" +
                        $"\n";

                    _log.LogInfo(message, LogProviderType.Console);
                }
            }
            _log.LogInfo($"Completed verification process, Total number of unique IDs reported in {scope} is: {count}.\n", LogProviderType.Console);
        }
    }
}
