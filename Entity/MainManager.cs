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

namespace Entity
{
    public class MainManager
    {
        private readonly AlarmScanner _alarmScanner;
        private readonly EventScanner _eventScanner;
        private readonly VariableScanner _variableScanner;
        public readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _log;

        public MainManager(AlarmScanner alarmScanner, EventScanner eventScanner, VariableScanner variableScanner, IUnitOfWork unitOfWork, LogManager log) 
        {
            _alarmScanner = alarmScanner;
            _eventScanner = eventScanner;
            _variableScanner = variableScanner;
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public bool ValidateXmlFilePaths(List<string> filePaths)
        {
            bool isValid = true;

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    _log.LogError($"Xml File Path {filePath} in Appsettings.json is not correct", LogProviderType.Console);
                    isValid = false; 
                }
            }

            return isValid;
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
                            CheckAllScopesForDuplicates(dataForDB);

                            return dataForDB;
                        }                   
                    }
                    else
                    {
                        _log.LogError($"{filePath} - File exist but is not an xml", LogProviderType.Console);
                    }
                }
                else
                {
                    _log.LogError($"{filePath} - Doesnt exist", LogProviderType.Console);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CheckAllScopesForDuplicates(SeperatedScopes dataForDb)
        {
            CheckForDuplicates(dataForDb.EventsList, "EventsList");
            CheckForDuplicates(dataForDb.AlarmsList, "AlarmsList");
            CheckForDuplicates(dataForDb.VariablesList, "VariablesList");
        }

        private void CheckForDuplicates(List<UniqueIds> list, string listName)
        {
            var duplicateNames = list.GroupBy(v => v.Name).Where(g => g.Count() > 1).Select(g => g.Key);
            var duplicateIDs = list.GroupBy(v => v.ID).Where(g => g.Count() > 1).Select(g => g.Key);

            LogDuplicates(listName, "names", duplicateNames);
            LogDuplicates(listName, "IDs", duplicateIDs);
        }

        private void LogDuplicates(string listName, string propertyName, IEnumerable<string> duplicates)
        {
            if (duplicates.Any())
            {
                string errorMessage = $"Duplicate {propertyName} found in {listName}: {string.Join(", ", duplicates)}";
                _log.LogError(errorMessage, LogProviderType.Console);
                _log.LogError(errorMessage, LogProviderType.File);
            }
        }

        public List<UniqueIds> RetriveUniqeIDsFromDB()
        {
            var result = _unitOfWork.UniqueIds.GetAll();     
            return (List<UniqueIds>) result;
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
            catch (Exception)
            {

                throw;
            }

        }

        public bool CompareXmlScopesWithDBScopes(SeperatedScopes xmlSeperatedScopes, SeperatedScopes DbSeperatedScopes)
        {
            return _alarmScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.AlarmsList, DbSeperatedScopes.AlarmsList) &&
                    _eventScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.EventsList, DbSeperatedScopes.EventsList) &&
                    _variableScanner.CompareXmlScopeWithDBScope(xmlSeperatedScopes.VariablesList, DbSeperatedScopes.VariablesList);
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
            UpdateDatabaseWithScanner(_alarmScanner);
            UpdateDatabaseWithScanner(_eventScanner);
            UpdateDatabaseWithScanner(_variableScanner);

            _unitOfWork.Complete();
        }

        private void UpdateDatabaseWithScanner(BaseScanner scanner)
        {
            var newIds = scanner.newUniqueIdsFromXml;

            if (newIds != null && newIds.Any())
            {
                _unitOfWork.UniqueIds.AddRange(newIds);
            }
        }
    }
}
