using Entity.EntityInterfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Scanners
{
    public class VariableScanner : IScanner
    {
        public Dictionary<string, M_UniqueIds> RetriveFromDB()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            Dictionary<string, M_UniqueIds> dataVariablesDictionary = new Dictionary<string, M_UniqueIds>();

            foreach (var datavar in ktgemvar.DataVariables)
            {
                string ID_KEY = datavar.Id.ToString();

                dataVariablesDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "DataVariable", ID = ID_KEY, Name = datavar.ExternalName, Scope = "Variables", Timestamp = DateTime.Now });
            }
            return dataVariablesDictionary;
        }
    }
}
