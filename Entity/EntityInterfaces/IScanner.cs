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
        List<UniqueIds> ScanCode(KlaXML ktgemvar);
        bool CompareXmlScopeWithDBScope(List<UniqueIds> xml, List<UniqueIds> db, bool getFullInfo);

    }
}
