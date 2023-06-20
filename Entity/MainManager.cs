using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Security.Claims;
using Entity.Scanners;
using Repository.Interfaces;
using Utility_LOG;
using Repository.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Entity
{
    public class MainManager
    {
        private readonly AlarmScanner _alarmScanner;
        private readonly EventScanner _eventScanner;
        private readonly VariableScanner _variableScanner;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _log;

        public MainManager(AlarmScanner alarmScanner, EventScanner eventScanner, VariableScanner variableScanner, IUnitOfWork unitOfWork, LogManager log) 
        {
            _alarmScanner = alarmScanner;
            _eventScanner = eventScanner;
            _variableScanner = variableScanner;
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public bool ValidateXmlFilePath(string filePath)
        {
            try
            {
                bool isValid = true;

                if (!File.Exists(filePath))
                {
                    _log.LogError($"Xml File Path {filePath} in Appsettings.json is not correct", LogProviderType.Console);
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

                if (File.Exists(filePath))
                {
                    if (Path.GetExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
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

                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ValidateAndPrepareAliases(Dictionary<string, string> renameInfo)
        {
            List<Aliases> newAliases = new List<Aliases>();

            var uniqueIdNames = _unitOfWork.UniqueIds.GetAll().Select(a => a.Name).ToList();

            var missingKeys = renameInfo.Keys.Except(uniqueIdNames).ToList();

            if (missingKeys.Any())
            {
                var keys = string.Join(", ", missingKeys);
                _log.LogError($"Keys '{keys}' not found in the database", LogProviderType.Console);

                throw new KeyNotFoundException($"Keys '{keys}' not found in the database");
            }

            // Fetching all existing aliases from the database and storing them in a HashSet for faster lookup
            var existingAliases = new HashSet<string>(_unitOfWork.Aliases.GetAll().Select(a => a.AliasName));

            var flattenedKeyUniqueIdPairs = renameInfo.Keys
                .SelectMany(key => _unitOfWork.UniqueIds.Find(a => a.Name == key)
                .Select(uniqueId => new { Key = key, UniqueId = uniqueId }));

            foreach (var pair in flattenedKeyUniqueIdPairs)
            {
                var newAlias = PrepareAliasIfNotExisting(pair.UniqueId, renameInfo[pair.Key], existingAliases);

                if (newAlias != null)
                {
                    newAliases.Add(newAlias);
                }
            }

            if(newAliases.Any())
            {
                UpdateDbWithNewAliases(newAliases);
            }
            
        }

        public Aliases PrepareAliasIfNotExisting(UniqueIds uniqueId, string aliasName, HashSet<string> existingAliases)
        {
            // Checking if alias already exists in the database
            if (!existingAliases.Contains(aliasName))
            {
                // If not, preparing new alias
                return new Aliases
                {
                    ID = uniqueId.ID,
                    OriginalName = uniqueId.Name,
                    AliasName = aliasName,
                    UniqueIdScope = uniqueId.Scope,
                    AliasCreated = DateTime.Now,

                };
            }
            else
            {
                // If alias already exists, logging a message
                _log.LogWarning($"Alias '{aliasName}' already exists in the database", LogProviderType.Console);
            }

            // If alias already exists, returning null
            return null;
        }

        public void UpdateDbWithNewAliases(List<Aliases> newAliases)
        {
            _unitOfWork.Aliases.AddRange(newAliases);
            _unitOfWork.Complete();

        }

    }
}
