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

namespace Entity
{
    public class MainManager
    {
        private VariableScanner _VariableScanner;
        private EventScanner _EventScanner;
        private AlarmScanner _AlarmScanner;
        public MainManager() 
        { 
            _VariableScanner = new VariableScanner();
            _EventScanner = new EventScanner();
            _AlarmScanner = new AlarmScanner();
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

                        dataForDB.Add("dataVariables", _VariableScanner.ScanCode(ktgemvar));
                        dataForDB.Add("events", _EventScanner.ScanCode(ktgemvar));
                        dataForDB.Add("alarms", _AlarmScanner.ScanCode(ktgemvar));
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
    }
}
