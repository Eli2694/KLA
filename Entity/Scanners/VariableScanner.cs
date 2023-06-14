using Entity.EntityInterfaces;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.Scanners
{
    public class VariableScanner : BaseScanner, IScanner
    {

        public VariableScanner(LogManager log) : base(log)
        {
        }

        public List<M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            try
            {
                var dataVariablesList = ktgemvar.DataVariables.Select(datavar =>
                    new M_UniqueIds
                    {
                        EntityType = "DataVariable",
                        ID = datavar.Id.ToString(),
                        Name = datavar.ExternalName,
                        Scope = "variable",
                        Timestamp = DateTime.Now
                    });

                var equipmentConstantsList = ktgemvar.EquipmentConstants.Select(equipment =>
                    new M_UniqueIds
                    {
                        EntityType = "EquipmentConstant",
                        ID = equipment.Id.ToString(),
                        Name = equipment.ExternalName,
                        Scope = "variable",
                        Timestamp = DateTime.Now
                    });

                var dynamicVariablesList = ktgemvar.DynamicVariables.Select(dynamic =>
                    new M_UniqueIds
                    {
                        EntityType = "DynamicVariable",
                        ID = dynamic.Id.ToString(),
                        Name = dynamic.ExternalName,
                        Scope = "variable",
                        Timestamp = DateTime.Now
                    });

                var statusVariablesList = ktgemvar.StatusVariables.Select(status =>
                    new M_UniqueIds
                    {
                        EntityType = "StatusVariable",
                        ID = status.Id.ToString(),
                        Name = status.ExternalName,
                        Scope = "variable",
                        Timestamp = DateTime.Now
                    });

                return dataVariablesList.Concat(equipmentConstantsList)
                                        .Concat(dynamicVariablesList)
                                        .Concat(statusVariablesList)
                                        .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
