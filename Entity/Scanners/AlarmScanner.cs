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

        public Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            Dictionary<string, M_UniqueIds> alaramsDictionary = new Dictionary<string, M_UniqueIds>();

            foreach (var alarm in ktgemvar.Alarms)
            {
                string ID_KEY = alarm.Id.ToString();
                alaramsDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "Alarm", ID = ID_KEY, Name = alarm.Name, Scope = "Alarms", Timestamp = DateTime.Now });
            }
            return alaramsDictionary;
        }
        public bool compareFileWithDB(Dictionary<string, M_UniqueIds> xml, Dictionary<string, M_UniqueIds> db)
        {
            foreach (var alarmDB in db)
            {
                foreach (var alarmXML in xml)
                {
                    if (alarmDB.Value.Name == alarmXML.Value.Name || alarmDB.Value.ID == alarmXML.Value.ID )
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //public Dictionary<string, M_UniqueIds> RetriveFromDB(List<M_UniqueIds> ListFromDB)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
