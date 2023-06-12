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
    public class VariableScanner : IScanner
    {

        //public Dictionary<string, M_UniqueIds> RetriveFromDB(List<M_UniqueIds> ListFromDB)
        //{

        //}

        public Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            Dictionary<string, M_UniqueIds> dataVariablesDictionary = new Dictionary<string, M_UniqueIds>();

            foreach (var datavar in ktgemvar.DataVariables)
            {
                string ID_KEY = datavar.Id.ToString();
                dataVariablesDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "DataVariable", ID = ID_KEY, Name = datavar.ExternalName, Scope = "Variables", Timestamp = DateTime.Now });
            }
            foreach (var Equipment in ktgemvar.EquipmentConstants)
            {
                string ID_KEY = Equipment.Id.ToString();
                dataVariablesDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "EquipmentConstant", ID = ID_KEY, Name = Equipment.ExternalName, Scope = "Variables", Timestamp = DateTime.Now });
            }

            foreach (var Dynamic in ktgemvar.DynamicVariables)
            {
                string ID_KEY = Dynamic.Id.ToString();
                dataVariablesDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "DynamicVariable", ID = ID_KEY, Name = Dynamic.ExternalName, Scope = "Variables", Timestamp = DateTime.Now });
            }

            foreach (var Status in ktgemvar.StatusVariables)
            {
                string ID_KEY = Status.Id.ToString();
                dataVariablesDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "StatusVariable", ID = ID_KEY, Name = Status.ExternalName, Scope = "Variables", Timestamp = DateTime.Now });
            }
            return dataVariablesDictionary;
        }

        public bool compareFileWithDB(Dictionary<string, M_UniqueIds> xml, Dictionary<string, M_UniqueIds> db)
        {
            foreach (var variableDB in db)
            {
                foreach (var variableXML in xml)
                {
                    if (variableDB.Value.Name == variableXML.Value.Name || variableDB.Value.ID == variableXML.Value.ID)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
