﻿using Model;
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

        // Dictionary<string, M_UniqueIds> RetriveFromDB(List< M_UniqueIds> ListFromDB);

        bool compareXmlScopeWithDBScope(List<M_UniqueIds> xml, List<M_UniqueIds> db);

    }
}
