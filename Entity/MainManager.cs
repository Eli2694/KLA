using Model;
using System.Xml.Serialization;
using System.Xml;
using Entity.Scanners;
using Repository.Interfaces;
using Utility_LOG;
using System.Text.Json;
using System.Text.Json.Serialization;
using Entity.EntityInterfaces;
using Model.XmlModels;
using Microsoft.EntityFrameworkCore;

namespace Entity
{
    public class MainManager
    {
        private readonly AlarmScanner _alarmScanner;
        private readonly EventScanner _eventScanner;
        private readonly VariableScanner _variableScanner;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _log;
        private readonly IFileSystem _fileSystem;

        public MainManager(AlarmScanner alarmScanner, EventScanner eventScanner, VariableScanner variableScanner, IUnitOfWork unitOfWork, LogManager log, IFileSystem fileSystem) 
        {
            _alarmScanner = alarmScanner;
            _eventScanner = eventScanner;
            _variableScanner = variableScanner;
            _unitOfWork = unitOfWork;
            _log = log;
            _fileSystem = fileSystem; 
        }

        public bool ValidateXmlFilePath(string filePath)
        {
            try
            {
                bool isValid = true;

                if (!_fileSystem.FileExists(filePath)) // Use the file system to check if the file exists
                {
                    _log.LogError($"Xml file path {filePath} is not valid", LogProviderType.Console);
                    isValid = false;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in ValidateXmlFilePath: {ex.Message}", LogProviderType.File);
                return false;
            }
        }

        public SeperatedScopes? XmlToSeperatedScopes(string filePath)
        {
            try
            {
                if (_fileSystem.GetFileExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    SeperatedScopes? dataForDB;

                    string rootElement = GetRootElementName(filePath);

                    if (rootElement != null)
                    {
                        switch (rootElement)
                        {
                            case "ktgem":
                                return dataForDB = KtgemSerializer(filePath);
                            default:
                                return null;
                        }
                    }

                }
                else
                {
                    _log.LogError($"{filePath} - Not Valid", LogProviderType.Console);
                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _log.LogError($"Exception In XmlToSeperatedScopes method: {ex.Message}", LogProviderType.File);
                throw;
            }
        }

        /* The purpose of this method is to determine the root element of the XML file,
         which is the first element encountered while reading through the XML content. */

        private string GetRootElementName(string filePath)
        {
            try
            {
                // Create an XmlReader to read the XML file
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        // Check if the current node is an element (start tag)
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            // Return the name of the first element node encountered
                            return reader.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Exception In GetRootElementName method: {ex.Message}", LogProviderType.File);
            }

            // Return null if no root element is found or an exception occurs
            return null;
        }


        private SeperatedScopes? KtgemSerializer(string filePath)
        {
            try
            {
                // Create an XmlSerializer to deserialize the XML file content into Ktgem object
                XmlSerializer serializer = new XmlSerializer(typeof(Ktgem));
                Ktgem? klaXml;

                // Read and deserialize the XML content
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    klaXml = (Ktgem?)serializer.Deserialize(reader);
                }

                if (klaXml != null)
                {
                    // Scan the Ktgem content and separate into different scope lists
                    SeperatedScopes dataForDB = new SeperatedScopes
                    {
                        VariablesList = _variableScanner.ScanKtgemContent(klaXml),
                        EventsList = _eventScanner.ScanKtgemContent(klaXml),
                        AlarmsList = _alarmScanner.ScanKtgemContent(klaXml)
                    };

                    // Check for duplicates in XML file 
                    if (CheckAllScopesForDuplicates(dataForDB))
                    {
                        return null;
                    }

                    return dataForDB;
                }

                return null;
            }
            catch (Exception ex)
            {
                _log.LogError($"Exception In KtgemSerializer method: {ex.Message}", LogProviderType.File);
                throw;
            }
        }

        public bool CheckAllScopesForDuplicates(SeperatedScopes dataForDb)
        {
            bool duplicatesFound = false;
            duplicatesFound |= CheckForDuplicates(dataForDb.EventsList, "EventsList");
            duplicatesFound |= CheckForDuplicates(dataForDb.AlarmsList, "AlarmsList");
            duplicatesFound |= CheckForDuplicates(dataForDb.VariablesList, "VariablesList");
            return duplicatesFound;
        }

        private bool CheckForDuplicates(List<UniqueIds> list, string listName)
        {
            try
            {
                // Group items in the list by Name and ID and identify duplicates
                var duplicateNames = list.GroupBy(v => v.Name).Where(g => g.Count() > 1).Select(g => g.Key);
                var duplicateIDs = list.GroupBy(v => v.ID).Where(g => g.Count() > 1).Select(g => g.Key);

                bool duplicatesFound = false;
                duplicatesFound |= LogDuplicates(listName, "names", duplicateNames);
                duplicatesFound |= LogDuplicates(listName, "IDs", duplicateIDs);

                return duplicatesFound;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in CheckForDuplicates method: {ex.Message}", LogProviderType.File);
                return false;
            }
        }

        private bool LogDuplicates(string listName, string propertyName, IEnumerable<string> duplicates)
        {
            int duplicatesCount = 0;
            if (duplicates.Any())
            {
                string errorMessage = $"Duplicate {propertyName} found in {listName}: {string.Join(", ", duplicates)}";
                _log.LogError(errorMessage, LogProviderType.Console);
                _log.LogError(errorMessage, LogProviderType.File);
                duplicatesCount++;
            }

            if(duplicatesCount > 0)
            {
                return true;
            }

            return false;
        }

        public List<UniqueIds> RetriveUniqeIDsFromDB()
        {
            try
            {
                var result = _unitOfWork.UniqueIds.GetAll();
                return (List<UniqueIds>)result;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in RetriveUniqeIDsFromDB method: {ex.Message}", LogProviderType.File);
                throw;
            }
        }

        public List<Aliases> RetriveAliasesFromDB()
        {
            try
            {
                var result = _unitOfWork.Aliases.GetAll();
                return (List<Aliases>)result;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in RetriveAliasesFromDB method: {ex.Message}", LogProviderType.File);
                throw;
            }
        }

        // Check if any names in the XML scopes exist in the alias lists from the database
        public bool CheckIfNamesExistInAliasLists(SeperatedScopes? xmlScopes)
        {
            try
            {
                var aliasesFromDb = RetriveAliasesFromDB();

                // Separate aliases by scope
                var eventAliases = aliasesFromDb.Where(a => a.Scope == "event").ToList();
                var alarmAliases = aliasesFromDb.Where(a => a.Scope == "alarm").ToList();
                var variableAliases = aliasesFromDb.Where(a => a.Scope == "variable").ToList();

                bool foundAlias = false; // Flag to track if any name is found as an alias

                if (xmlScopes != null)
                {
                    // Check if names in EventsList exist in eventAliases
                    if (xmlScopes.EventsList != null)
                    {
                        foreach (var eventId in xmlScopes.EventsList)
                        {
                            var eventName = eventId.Name;

                            if (eventAliases.Any(a => a.CurrentAliasName == eventName))
                            {
                                _log.LogError($"Event name '{eventName}' was found as an alias", LogProviderType.Console);
                                _log.LogError($"Event name '{eventName}' was found as an alias", LogProviderType.File);
                                foundAlias = true;
                            }
                        }
                    }

                    // Check if names in AlarmsList exist in alarmAliases
                    if (xmlScopes.AlarmsList != null)
                    {
                        foreach (var alarm in xmlScopes.AlarmsList)
                        {
                            var alarmName = alarm.Name;

                            if (alarmAliases.Any(a => a.CurrentAliasName == alarmName))
                            {
                                _log.LogError($"Alarm name '{alarmName}' was found as an alias", LogProviderType.Console);
                                _log.LogError($"Alarm name '{alarmName}' was found as an alias", LogProviderType.File);
                                foundAlias = true;
                            }
                        }
                    }

                    // Check if names in VariablesList exist in variableAliases
                    if (xmlScopes.VariablesList != null)
                    {
                        foreach (var variable in xmlScopes.VariablesList)
                        {
                            var variableName = variable.Name;

                            if (variableAliases.Any(a => a.CurrentAliasName == variableName))
                            {
                                _log.LogError($"Variable name '{variableName}' was found as an alias", LogProviderType.Console);
                                _log.LogError($"Variable name '{variableName}' was found as an alias", LogProviderType.File);
                                foundAlias = true;
                            }
                        }
                    }
                }

                // Return false if any name was found as an alias, otherwise return true
                return !foundAlias;
            }
            catch (Exception ex)
            {
                _log.LogError($"CheckIfNamesExistInAliasLists method: {ex.Message}", LogProviderType.Console);
                _log.LogError($"CheckIfNamesExistInAliasLists method: {ex.Message}", LogProviderType.File);
                throw;
            }
            
        }

        // Sorts UniqueIds from the database into separate scopes
        public SeperatedScopes SortUniqeIDsFromDbByScope(List<UniqueIds> ListFromDB)
        {
            try
            {
                SeperatedScopes DbInObjects = new SeperatedScopes();

                foreach (var obj in ListFromDB)
                {
                    switch (obj.Scope)
                    {
                        case "event":
                            DbInObjects.EventsList.Add(obj);
                            break;
                        case "alarm":
                            DbInObjects.AlarmsList.Add(obj);
                            break;
                        case "variable":
                            DbInObjects.VariablesList.Add(obj);
                            break;
                    }
                }
                return DbInObjects;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in SortUniqeIDsFromDbByScope method: {ex.Message}", LogProviderType.File);
                throw;
            }

        }

        // Compares XML scopes with database scopes
        public bool CompareXmlScopesWithDBScopes(SeperatedScopes xmlSeperatedScopes, SeperatedScopes DbSeperatedScopes, bool getFullInfo, string filePath)
        {
            try
            {

                string fileName = Path.GetFileName(filePath);
                _log.LogEvent($"Verifying File: {fileName}...", LogProviderType.Console);

                // Compare each scope's objects between XML and the database
                return _alarmScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.AlarmsList, DbSeperatedScopes.AlarmsList, getFullInfo) &&
                        _eventScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.EventsList, DbSeperatedScopes.EventsList, getFullInfo) &&
                        _variableScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.VariablesList, DbSeperatedScopes.VariablesList, getFullInfo);
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in CompareXmlScopesWithDBScopes method: {ex.Message}", LogProviderType.File);
                return false;
            }
        }

        public void UpdateDatabaseWithNewUniqueIds()
        {
            try
            {

                _unitOfWork.UniqueIds.DetachAll();

                UpdateDatabaseWithScanner(_alarmScanner);
                UpdateDatabaseWithScanner(_eventScanner); 
                UpdateDatabaseWithScanner(_variableScanner);

                _unitOfWork.Complete();

                _log.LogEvent($"The database update process is finished!", LogProviderType.Console);

  
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            { 
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in UpdateDatabaseWithNewUniqueIds: {ex.Message}", LogProviderType.File);
            }
        }

        private void UpdateDatabaseWithScanner(BaseScanner scanner)
        {
            try
            {
                var newIds = scanner.newUniqueIdsFromXml;

                if (newIds != null && newIds.Any())
                {
                    _unitOfWork.UniqueIds.AddRange(newIds);
                    scanner.newUniqueIdsFromXml.Clear();
                    
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in UpdateDatabaseWithScanner: {ex.Message}", LogProviderType.File);
                throw;
            }
            
        }


        public bool isAuthenticatedUser(List<string> NameAndPass)
        {
            _log.LogEvent($"Authenticating User....", LogProviderType.Console);

            User user = _unitOfWork.Users.GetValidatedUser(NameAndPass[0]);
            if (user != null)
            {
                return user.Password == NameAndPass[1];
            }
            return false;
        }

        public void GenerateReport(string filePath)
        {
            try
            {

                var uniqueIdWithAliases =  _unitOfWork.UniqueIds.GetUniqueIdsWithAliases();

                // not using ReferenceHandler.Preserve can cause an infinite loop 
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(uniqueIdWithAliases, options);

                _log.LogEvent($"Generating Report Of Unique Ids....", LogProviderType.Console);

                _fileSystem.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
        // Handle Aliases

        public void ValidateAndPrepareAliases(Dictionary<string, string> renameInfo)
        {

            try
            {
                // Get All Names From UniqueIds And Aliases Tables
                var listOfAllNames = ListOfAllNamesFromAllTablesInDB();

                // Checking If Rename Keys Exists In Database
                var missingKeys = renameInfo.Keys.Except(listOfAllNames).ToList();

                if (missingKeys.Any())
                {
                    var keys = string.Join(", ", missingKeys);
                    _log.LogError($"Keys '{keys}' not found in the database", LogProviderType.Console);

                    throw new KeyNotFoundException($"Keys '{keys}' not found in the database");
                }

                // Check If Aliases Already Exists In UniqueIds Table
                bool isAliasFound = CheckIfAliasFoundInUniqueIds(renameInfo);
                if (isAliasFound)
                {
                    throw new Exception("Aliases already exists in UniqueIds table.");
                }

                // Retrieve all current alias names from the Aliases table in the database and store them in a HashSet.
                // This HashSet will be used to efficiently check if a new alias is already present in the Aliases table.
                var existingAliases = new HashSet<string>(_unitOfWork.Aliases.GetAll().Select(a => a.CurrentAliasName));


                // Retrieve detailed information about each key from various tables in the database
                var keyList = renameInfo.Keys.ToList();
                var fullInformationAboutEveryKey = GetKeyInfoFromAllTablesInDB(keyList);

                // Prepare to create new aliases for inserting to database
                var newAliases = PrepareAndCreateAliases(fullInformationAboutEveryKey, renameInfo, existingAliases);


                if (newAliases.Any())
                {
                    _log.LogEvent($"Updating Database With New Aliases...", LogProviderType.Console);
                    UpdateDbWithNewAliases(newAliases);
                    _log.LogEvent($"Database Was Updated With New Aliases", LogProviderType.Console);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in SetUpRename method: {ex.Message}", LogProviderType.File);
                throw;
            }   
        }

        private List<Aliases> PrepareAndCreateAliases(List<object> keyInfoList, Dictionary<string, string> renameInfo, HashSet<string> existingAliases)
        {
            List<Aliases> newAliases = new List<Aliases>();

            foreach (var keyInfo in keyInfoList)
            {
                try
                {
                    if (keyInfo is UniqueIds uniqueIdInfo)
                    {
                        Aliases newAlias = PrepareAliasIfNotExisting(uniqueIdInfo.ID, uniqueIdInfo.Name, uniqueIdInfo.Scope, renameInfo[uniqueIdInfo.Name], existingAliases);
                        if (newAlias != null)
                        {
                            newAliases.Add(newAlias);
                        }
                    }
                    else if (keyInfo is Aliases aliasInfo)
                    {
                        Aliases newAlias = PrepareAliasIfNotExisting(aliasInfo.ID, aliasInfo.PreviousAliasName, aliasInfo.Scope, renameInfo[aliasInfo.PreviousAliasName], existingAliases);
                        if (newAlias != null)
                        {
                            newAliases.Add(newAlias);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError($"An error occurred while preparing alias: {ex.Message}", LogProviderType.File);
                }
            }

            return newAliases;
        }

        public Aliases PrepareAliasIfNotExisting(string id, string previousName, string scope, string newAlias, HashSet<string> existingAliases)
        {
            try
            {
                // Checking if alias already exists in the database
                if (!existingAliases.Contains(newAlias))
                {
                    // If not, preparing new alias
                    return new Aliases
                    {
                        ID = id,
                        PreviousAliasName = previousName,
                        CurrentAliasName = newAlias,
                        Scope = scope,
                        AliasCreated = DateTime.Now,
                    };
                }
                else
                {
                    // If alias already exists, logging a message
                    _log.LogWarning($"Alias '{newAlias}' already exists in the database", LogProviderType.Console);
                }

                // If alias already exists, returning null
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in PrepareAliasIfNotExisting method  : {ex.Message}", LogProviderType.File);
                throw;
            }
      
        }

        public List<string> ListOfAllNamesFromAllTablesInDB()
        {
            try
            {
                var uniqueIdNames = _unitOfWork.UniqueIds.GetAll().Select(a => a.Name);
                var previousAliasNames = _unitOfWork.Aliases.GetAll().Select(a => a.PreviousAliasName);
                var currentAliasNames = _unitOfWork.Aliases.GetAll().Select(a => a.CurrentAliasName);

                return uniqueIdNames.Concat(previousAliasNames).Concat(currentAliasNames).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool CheckIfAliasFoundInUniqueIds(Dictionary<string, string> renameInfo)
        {

            try
            {
                var uniqueIdNames = _unitOfWork.UniqueIds.GetAll().Select(a => a.Name);
                var foundAliases = renameInfo.Values.Intersect(uniqueIdNames).ToList();

                foreach (var alias in foundAliases)
                {
                    _log.LogError($"Alias '{alias}' found in UniqueIds Table.", LogProviderType.Console);
                }

                return foundAliases.Count > 0;
            }
            catch (Exception)
            {

                throw;
            }   
        }
        public List<object> GetKeyInfoFromAllTablesInDB(List<string> keys)
        {
            try
            {
                List<object> keyInfoList = new List<object>();

                foreach (var key in keys)
                {
                    // Retrieve information from UniqueIds table
                    var uniqueIdInfo = _unitOfWork.UniqueIds.GetAll().FirstOrDefault(a => a.Name == key);
                    if (uniqueIdInfo != null)
                    {
                        keyInfoList.Add(new UniqueIds
                        {
                            ID = uniqueIdInfo.ID,
                            Name = key,
                            Scope = uniqueIdInfo.Scope
                        });
                    }
                    else
                    {
                        // Retrieve information from Aliases table
                        var aliasInfo = _unitOfWork.Aliases.GetAll().FirstOrDefault(a => a.PreviousAliasName == key || a.CurrentAliasName == key);
                        if (aliasInfo != null)
                        {
                            keyInfoList.Add(new Aliases
                            {
                                ID = aliasInfo.ID,
                                PreviousAliasName = key,
                                Scope = aliasInfo.Scope
                            });
                        }
                    }
                }

                return keyInfoList;
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in GetKeyInfoFromAllTablesInDB method: {ex.Message}", LogProviderType.File);
                throw;
            }

            
        }

        public void UpdateDbWithNewAliases(List<Aliases> newAliases)
        {
            try
            {
                _unitOfWork.Aliases.AddRange(newAliases);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in UpdateDbWithNewAliases: {ex.Message}", LogProviderType.File);
                throw;
            }
            
        }

    }
}
