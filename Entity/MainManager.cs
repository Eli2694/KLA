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

        public M_SeperatedScopes? XmlToSeperatedScopes(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(M_KlaXML));
                    M_KlaXML? klaXml;

                    using (XmlReader reader = XmlReader.Create(filePath))
                    {
                        klaXml = (M_KlaXML?)serializer.Deserialize(reader);
                    }

                    if (klaXml != null)
                    {
                        M_SeperatedScopes dataForDB = new M_SeperatedScopes();

                        dataForDB.VariablesList = _variableScanner.ScanCode(klaXml);
                        dataForDB.EventsList = _eventScanner.ScanCode(klaXml);
                        dataForDB.AlarmsList = _alarmScanner.ScanCode(klaXml);
                        CheckAllScopesForDuplicates(dataForDB);

                        return dataForDB;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CheckAllScopesForDuplicates(M_SeperatedScopes dataForDb)
        {
            CheckForDuplicates(dataForDb.EventsList, "EventsList");
            CheckForDuplicates(dataForDb.AlarmsList, "AlarmsList");
            CheckForDuplicates(dataForDb.VariablesList, "VariablesList");
        }

        private void CheckForDuplicates(List<M_UniqueIds> list, string listName)
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

        public List<M_UniqueIds> RetriveUniqeIDsFromDB()
        {
            var result = _unitOfWork.UniqueIds.GetAll();     
            return (List<M_UniqueIds>) result;
        }

        public M_SeperatedScopes SortUniqeIDsFromDbByScope(List<M_UniqueIds> ListFromDB)
        {
            try
            {
                M_SeperatedScopes DbInObjects = new M_SeperatedScopes();

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

        public bool CompareXmlScopesWithDBScopes(M_SeperatedScopes xmlSeperatedScopes, M_SeperatedScopes DbSeperatedScopes)
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
