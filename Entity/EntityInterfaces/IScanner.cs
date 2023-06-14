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
        List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar);
        bool CompareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db);

    }
}
