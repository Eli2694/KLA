using Model;
using Model.XmlModels;

namespace Entity.EntityInterfaces
{
    public interface IScanner
    {
        List<UniqueIds> ScanKtgemContent(Ktgem ktgemvar);
        bool CompareXmlScopeWithDBScope(List<UniqueIds> xml, List<UniqueIds> db, bool getFullInfo);

    }
}
