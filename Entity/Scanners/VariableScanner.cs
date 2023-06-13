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

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            try
            {             
            List<M_UniqueIds> dataVariablesList = new List<M_UniqueIds>();

            foreach (var datavar in ktgemvar.DataVariables)
            {
                dataVariablesList.Add( new M_UniqueIds { EntityType = "DataVariable", ID = datavar.Id.ToString() , Name = datavar.ExternalName, Scope = "variable", Timestamp = DateTime.Now });
            }
            foreach (var Equipment in ktgemvar.EquipmentConstants)
            {
                dataVariablesList.Add( new M_UniqueIds { EntityType = "EquipmentConstant", ID = Equipment.Id.ToString() ,  Name = Equipment.ExternalName, Scope = "variable", Timestamp = DateTime.Now });
            }

            foreach (var Dynamic in ktgemvar.DynamicVariables)
            {
                dataVariablesList.Add( new M_UniqueIds { EntityType = "DynamicVariable", ID = Dynamic.Id.ToString() ,  Name = Dynamic.ExternalName, Scope = "variable", Timestamp = DateTime.Now });
            }

            foreach (var Status in ktgemvar.StatusVariables)
            {
                dataVariablesList.Add( new M_UniqueIds { EntityType = "StatusVariable", ID = Status.Id.ToString() ,  Name = Status.ExternalName, Scope = "variable", Timestamp = DateTime.Now });
            }
            return dataVariablesList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool compareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db)
        {
            return !db.Any(variableDB => XmlContainsVariable(xml, variableDB));
        }
        private bool XmlContainsVariable(List<M_UniqueIds> xml, M_UniqueIds variableDB)
        {
            return xml.Any(variableXML => variableDB.Name == variableXML.Name || variableDB.ID == variableXML.ID);
        }
    }
}
