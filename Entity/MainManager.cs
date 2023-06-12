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

namespace Entity
{
    public class MainManager
    {
        private readonly AlarmScanner _alarmScanner;
        private readonly EventScanner _eventScanner;
        private readonly VariableScanner _variableScanner;
        private readonly IUnitOfWork _unitOfWork;

        public MainManager(AlarmScanner alarmScanner, EventScanner eventScanner, VariableScanner variableScanner, IUnitOfWork unitOfWork) 
        {
            _alarmScanner = alarmScanner;
            _eventScanner = eventScanner;
            _variableScanner = variableScanner;
            _unitOfWork = unitOfWork;
        }
        
        public Dictionary<string, Dictionary<string, M_UniqueIds>>? XmlToObjectsDictionary(string filePath)
        {
            try
            {
                //string path = @"E:/CodingPlayground/XMLSerializerExmaple/XmlSerizalizeExample/XmlSerizalizeExample/bin/Debug/net6.0/ATLAS.reassign.xml";
                if (File.Exists(filePath))
                {

                    XmlSerializer ser = new XmlSerializer(typeof(M_KlaXML));
                    M_KlaXML? ktgemvar;

                    using (XmlReader reader = XmlReader.Create(filePath))
                    {
                        ktgemvar = (M_KlaXML?)ser.Deserialize(reader);
                    }

                    //if extracted the data successfully from the xml file to object
                    if (ktgemvar != null)
                    {
                        Dictionary<string, Dictionary<string, M_UniqueIds>> dataForDB = new Dictionary<string, Dictionary<string, M_UniqueIds>>();

                        //maybe add threads
                        dataForDB.Add("variables", _variableScanner.ScanCode(ktgemvar));
                        dataForDB.Add("events", _eventScanner.ScanCode(ktgemvar));
                        dataForDB.Add("alarms", _alarmScanner.ScanCode(ktgemvar));
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

        public List<M_UniqueIds> RetriveUniqeIDsFromDB()
        {
            var result = _unitOfWork.UniqueIds.GetAll();     
            return (List<M_UniqueIds>) result;
        }

        public Dictionary<string, Dictionary<string, M_UniqueIds>> SortUniqeIDsFromDbByScope(List<M_UniqueIds> ListFromDB)
        {
            try
            {
                Dictionary<string, Dictionary<string, M_UniqueIds>> DbInObjects = new Dictionary<string, Dictionary<string, M_UniqueIds>>();
                Dictionary<string, M_UniqueIds> EventsDictionary = new Dictionary<string, M_UniqueIds>();
                Dictionary<string, M_UniqueIds> AlarmsDictionary = new Dictionary<string, M_UniqueIds>();
                Dictionary<string, M_UniqueIds> VariableDictionary = new Dictionary<string, M_UniqueIds>();

                //HashSet<M_UniqueIds> EventsDictionary = new HashSet<M_UniqueIds>();

                foreach (var obj in ListFromDB)
                {
                    switch (obj.Scope)
                    {
                        case "event":
                            EventsDictionary.Add(obj.ID, obj);
                            break;
                        case "alarm":
                            AlarmsDictionary.Add(obj.ID, obj);
                            break;
                        case "variable":
                            VariableDictionary.Add(obj.ID, obj);
                            break;
                    }
                }
                DbInObjects.Add("variables", VariableDictionary);
                DbInObjects.Add("events", EventsDictionary);
                DbInObjects.Add("alarms", AlarmsDictionary);

                return DbInObjects;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
