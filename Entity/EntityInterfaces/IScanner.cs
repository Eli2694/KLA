using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.EntityInterfaces
{
    public interface IScanner
    {
        Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar);

        Dictionary<string, M_UniqueIds> RetriveFromDB();
    }
}
