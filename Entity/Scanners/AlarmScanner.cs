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
            M_ComparedLists comparedLists = new M_ComparedLists();

            foreach (var dbVariable in db)
            {
                compareXmlScopeWithDBScope(xml, dbVariable, comparedLists);
            }
            if (comparedLists.matchingIdAndDifferentName.Count == 0 && comparedLists.differentIdAndMatchingName.Count ==0) { return true; }
            return false;
        }


        private void compareXmlScopeWithDBScope(List<M_UniqueIds> xml, M_UniqueIds dbVariable, M_ComparedLists comparedLists)
        {

            foreach (var xmlVariable in xml)
            {
                if (dbVariable.ID == xmlVariable.ID)
                {
                    if (dbVariable.Name == xmlVariable.Name)
                    {
                        comparedLists.matchingIdAndName.Add(xmlVariable);
                    }
                    else
                    {
                        comparedLists.matchingIdAndDifferentName.Add(xmlVariable);
                    }
                }
                else
                {
                    if (dbVariable.Name == xmlVariable.Name)
                    {
                        comparedLists.differentIdAndMatchingName.Add(xmlVariable);
                    }
                    else
                    {
                        comparedLists.differentIdAndDifferentName.Add(xmlVariable);
                    }
                }
            }
        }

    }
}
