using Entity.EntityInterfaces;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Scanners
{
    public class AlarmScanner : IScanner
    {

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            List<M_UniqueIds> alaramsDictionary = new List<M_UniqueIds>();

            foreach (var alarm in ktgemvar.Alarms)
            {
                alaramsDictionary.Add( new M_UniqueIds { EntityType = "Alarm", ID = alarm.Id.ToString(), Name = alarm.Name, Scope = "alarm", Timestamp = DateTime.Now });
            }
            return alaramsDictionary;
        }
        public bool compareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db)
        {
            return !db.Any(variableDB => XmlContainsVariable(xml, variableDB));
        }
        private bool XmlContainsVariable(List<M_UniqueIds> xml, M_UniqueIds variableDB)
        {
            return xml.Any(variableXML => variableDB.Name == variableXML.Name || variableDB.ID == variableXML.ID);
        }

        //public Dictionary<string, M_UniqueIds> RetriveFromDB(List<M_UniqueIds> ListFromDB)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
