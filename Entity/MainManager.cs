using Model;
using System.Xml.Serialization;
using System.Xml;
using Entity.Scanners;
using Repository.Interfaces;
using Utility_LOG;
using System.Text.Json;
using System.Text.Json.Serialization;
using Entity.EntityInterfaces;

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

                if (_fileSystem.FileExists(filePath))
                {
                    if (_fileSystem.GetFileExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(KlaXML));
                        KlaXML? klaXml;

                        using (XmlReader reader = XmlReader.Create(filePath))
                        {
                            klaXml = (KlaXML?)serializer.Deserialize(reader);
                        }

                        if (klaXml != null)
                        {
                            SeperatedScopes dataForDB = new SeperatedScopes();

                            dataForDB.VariablesList = _variableScanner.ScanCode(klaXml);
                            dataForDB.EventsList = _eventScanner.ScanCode(klaXml);
                            dataForDB.AlarmsList = _alarmScanner.ScanCode(klaXml);
                            
                            if (CheckAllScopesForDuplicates(dataForDB))
                            {
                                 return null;
                            }

                            return dataForDB;
                        }                   
                    }
                    else
                    {
                        _log.LogError($"{filePath} - Not Valid", LogProviderType.Console);
                        return null;
                    }
       
                }
                else
                {
                    _log.LogError($"{filePath} - Not Found", LogProviderType.Console);
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
                            DbInObjects.EventsList.Add( obj);
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

        public bool CompareXmlScopesWithDBScopes(SeperatedScopes xmlSeperatedScopes, SeperatedScopes DbSeperatedScopes, bool getFullInfo)
        {
            try
            {
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

        //public async Task<bool> CompareXmlScopesWithDBScopes(M_SeperatedScopes xmlSeperatedScopes, M_SeperatedScopes DbSeperatedScopes)
        //{
        //    var taskAlarm = Task.Run(() => _alarmScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.AlarmsList, DbSeperatedScopes.AlarmsList));
        //    var taskEvent = Task.Run(() => _eventScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.EventsList, DbSeperatedScopes.EventsList));
        //    var taskVariable = Task.Run(() => _variableScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.VariablesList, DbSeperatedScopes.VariablesList));

        //    var results = await Task.WhenAll(taskAlarm, taskEvent, taskVariable);

        //    return results.All(r => r);
        //}

        public void UpdateDatabaseWithNewUniqueIds()
        {
            try
            {
                UpdateDatabaseWithScanner(_alarmScanner);
                UpdateDatabaseWithScanner(_eventScanner);
                UpdateDatabaseWithScanner(_variableScanner);

                _unitOfWork.Complete();

                _log.LogEvent($"The database was updated in the 'Unique_Ids' table using new IDs from an XML file.", LogProviderType.Console);

            }
            catch (Exception ex)
            {
                _log.LogError($"An error occurred in UpdateDatabaseWithNewUniqueIds: {ex.Message}", LogProviderType.File);
            }
        }

        private void UpdateDatabaseWithScanner(BaseScanner scanner)
        {

            var newIds = scanner.newUniqueIdsFromXml;

            if (newIds != null && newIds.Any())
            {
                _unitOfWork.UniqueIds.AddRange(newIds);
                scanner.newUniqueIdsFromXml.Clear();
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

        public List<string> ListOfAllNamesFromAllTablesInDB()
        {
            List<string> allNames = new List<string>();

            List<string> uniqueIdNames = _unitOfWork.UniqueIds.GetAll().Select(a => a.Name).ToList();
            List<string> previousAliasNames = _unitOfWork.Aliases.GetAll().Select(a => a.AliasPreviousName).ToList();
            List<string> currentAliasNames = _unitOfWork.Aliases.GetAll().Select(a => a.AliasCurrentName).ToList();

            allNames.AddRange(uniqueIdNames);
            allNames.AddRange(previousAliasNames);
            allNames.AddRange(currentAliasNames);

            return allNames;
        }

        public void ValidateAndPrepareAliases(Dictionary<string, string> renameInfo)
        {
            List<Aliases> newAliases = new List<Aliases>();

            List<string> listOfAllNames = ListOfAllNamesFromAllTablesInDB();

            var missingKeys = renameInfo.Keys.Except(listOfAllNames).ToList();

            if (missingKeys.Any())
            {
                var keys = string.Join(", ", missingKeys);
                _log.LogError($"Keys '{keys}' not found in the database", LogProviderType.Console);

                throw new KeyNotFoundException($"Keys '{keys}' not found in the database");
            }

            // Fetching all existing aliases from the database and storing them in a HashSet for faster lookup
            var existingAliases = new HashSet<string>(_unitOfWork.Aliases.GetAll().Select(a => a.AliasCurrentName));

            // if i can find the key, i want to get all the information about it from the database
            List<string> keyList = renameInfo.Keys.ToList();
            var fullInformationAboutEveryKey = GetKeyInfoFromAllTablesInDB(keyList);


            foreach (var keyInfo in fullInformationAboutEveryKey)
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
                    Aliases newAlias = PrepareAliasIfNotExisting(aliasInfo.ID, aliasInfo.AliasPreviousName, aliasInfo.Scope, renameInfo[aliasInfo.AliasPreviousName], existingAliases);
                    if (newAlias != null)
                    {
                        newAliases.Add(newAlias);
                    }
                }
            }

            if (newAliases.Any())
            {
                _log.LogEvent($"Updating Database With New Aliases", LogProviderType.Console);
                UpdateDbWithNewAliases(newAliases);
            }

        }

        public List<object> GetKeyInfoFromAllTablesInDB(List<string> keys)
        {
            List<object> keyInfoList = new List<object>();

            foreach (var key in keys)
            {
                // Retrieve information from UniqueIds table
                var uniqueIdInfo = _unitOfWork.UniqueIds.GetAll().FirstOrDefault(a => a.Name == key);
                if (uniqueIdInfo != null)
                {

                    UniqueIds uniqueIdKeyInfo = new UniqueIds
                    {
                        ID = uniqueIdInfo.ID,
                        Name = key,
                        Scope = uniqueIdInfo.Scope
                    };
                    keyInfoList.Add(uniqueIdKeyInfo);
                }

                // Retrieve information from Aliases table (AliasPreviousName)
                var aliasPrevInfo = _unitOfWork.Aliases.GetAll().FirstOrDefault(a => a.AliasPreviousName == key || a.AliasCurrentName == key);
                if (aliasPrevInfo != null)
                {
                    var aliasKeyInfo = new Aliases
                    {
                        ID = aliasPrevInfo.ID,
                        AliasPreviousName = key,
                        Scope = aliasPrevInfo.Scope
                    };
                    
                    keyInfoList.Add(aliasKeyInfo);
                }      
            }

            return keyInfoList;
        }

        public Aliases PrepareAliasIfNotExisting(string id,string previousName, string scope, string newAlias, HashSet<string> existingAliases)
        {
            // Checking if alias already exists in the database
            if (!existingAliases.Contains(newAlias))
            {
                // If not, preparing new alias
                return new Aliases
                {
                    ID = id,
                    AliasPreviousName = previousName,
                    AliasCurrentName = newAlias,
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
