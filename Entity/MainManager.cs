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
        
        public M_SeperatedScopes? XmlToObjectsDictionary(string filePath)
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
                        M_SeperatedScopes dataForDB = new M_SeperatedScopes();

                        //maybe add threads
                        dataForDB.VariableDictionary = _variableScanner.ScanCode(ktgemvar);
                        dataForDB.EventsDictionary = _eventScanner.ScanCode(ktgemvar);
                        dataForDB.AlarmsDictionary = _alarmScanner.ScanCode(ktgemvar);
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

        public M_SeperatedScopes SortUniqeIDsFromDbByScope(List<M_UniqueIds> ListFromDB)
        {
            try
            {
                M_SeperatedScopes DbInObjects = new M_SeperatedScopes();
                //HashSet<M_UniqueIds> EventsDictionary = new HashSet<M_UniqueIds>();

                foreach (var obj in ListFromDB)
                {
                    switch (obj.Scope)
                    {
                        case "event":
                            DbInObjects.EventsDictionary.Add(obj.ID, obj);
                            break;
                        case "alarm":
                            DbInObjects.AlarmsDictionary.Add(obj.ID, obj);
                            break;
                        case "variable":
                            DbInObjects.VariableDictionary.Add(obj.ID, obj);
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

        public bool compareXmlScopesWithDBScopes(M_SeperatedScopes xmlAsDictionary, M_SeperatedScopes DbAsDictionary)
        {

            if (_alarmScanner.compareXmlScopeWithDBScope(xmlAsDictionary.AlarmsDictionary,DbAsDictionary.AlarmsDictionary))
            {
                if (_eventScanner.compareXmlScopeWithDBScope(xmlAsDictionary.EventsDictionary,DbAsDictionary.EventsDictionary))
                {
                    if (_variableScanner.compareXmlScopeWithDBScope(xmlAsDictionary.VariableDictionary,DbAsDictionary.VariableDictionary))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
