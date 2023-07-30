using Entity.Scanners;
using Model;
using Model.XmlModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.UnitTest.ScannersTest
{
    [NonParallelizable]
    [TestFixture]
    public class VariableScannerTests
    {
        //private Mock<LogManager> _mockLogManager;
        private VariableScanner _target;

        [SetUp]
        public void Setup()
        {
            //_mockLogManager = new Mock<LogManager>();
            _target = new VariableScanner();
        }

        private void AssertMappedUniqueId(UniqueIds uniqueId, string id, string entityType, string name, string scope)
        {
            Assert.IsNotNull(uniqueId, $"Mapped M_UniqueIds object for {entityType} {id} should not be null");
            Assert.AreEqual(entityType, uniqueId.EntityType, $"EntityType for {entityType} {id} should be set to '{entityType}'");
            Assert.AreEqual(name, uniqueId.Name, $"Name for {entityType} {id} should be mapped correctly");
            Assert.AreEqual(scope, uniqueId.Scope, $"Scope for {entityType} {id} should be set to '{scope}'");
        }

        [Test]
        public void ScanCode_MapsVariableDataCorrectly_WhenVariablesExist()
        {
            // Arrange
            var dataVariables = new List<DataVariable>
            {
                new DataVariable { Id = 1, ExternalName = "DataVariable 1" },
                new DataVariable { Id = 2, ExternalName = "DataVariable 2" },
                new DataVariable { Id = 3, ExternalName = "DataVariable 3" }
            };

            var equipmentConstants = new List<EquipmentConstant>
            {
                new EquipmentConstant { Id = 4, ExternalName = "EquipmentConstant 1" },
                new EquipmentConstant { Id = 5, ExternalName = "EquipmentConstant 2" }
            };

            var dynamicVariables = new List<DynamicVariable>
            {
                new DynamicVariable { Id = 6, ExternalName = "DynamicVariable 1" },
                new DynamicVariable { Id = 7, ExternalName = "DynamicVariable 2" },
                new DynamicVariable { Id = 8, ExternalName = "DynamicVariable 3" }
            };

            var statusVariables = new List<StatusVariable>
            {
                new StatusVariable { Id = 9, ExternalName = "StatusVariable 1" }
            };

            var ktgemvar = CreateKtgemvarWithVariables(dataVariables, equipmentConstants, dynamicVariables, statusVariables);

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.Multiple(() =>
            {
                // DataVariables
                foreach (var dataVar in dataVariables)
                {
                    UniqueIds uniqueId = result.FirstOrDefault(u => u.ID == dataVar.Id.ToString());
                    AssertMappedUniqueId(uniqueId, dataVar.Id.ToString(), "DataVariable", dataVar.ExternalName, "variable");
                }

                // EquipmentConstants
                foreach (var equipment in equipmentConstants)
                {
                    UniqueIds uniqueId = result.FirstOrDefault(u => u.ID == equipment.Id.ToString());
                    AssertMappedUniqueId(uniqueId, equipment.Id.ToString(), "EquipmentConstant", equipment.ExternalName, "variable");
                }

                // DynamicVariables
                foreach (var dynamicVar in dynamicVariables)
                {
                    UniqueIds uniqueId = result.FirstOrDefault(u => u.ID == dynamicVar.Id.ToString());
                    AssertMappedUniqueId(uniqueId, dynamicVar.Id.ToString(), "DynamicVariable", dynamicVar.ExternalName, "variable");
                }

                // StatusVariables
                foreach (var statusVar in statusVariables)
                {
                    UniqueIds uniqueId = result.FirstOrDefault(u => u.ID == statusVar.Id.ToString());
                    AssertMappedUniqueId(uniqueId, statusVar.Id.ToString(), "StatusVariable", statusVar.ExternalName, "variable");
                }
            });
        }

        [Test]
        public void ScanCode_ReturnsListOfMUniqueIds_WhenVariablesExist()
        {
            // Arrange
            var ktgemvar = CreateKtgemvarWithVariables(
                new List<DataVariable> { new DataVariable { Id = 1, ExternalName = "DataVariable 1" } },
                new List<EquipmentConstant> { new EquipmentConstant { Id = 1, ExternalName = "EquipmentConstant 1" } },
                new List<DynamicVariable> { new DynamicVariable { Id = 1, ExternalName = "DynamicVariable 1" } },
                new List<StatusVariable> { new StatusVariable { Id = 1, ExternalName = "StatusVariable 1" } }
            );

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.IsInstanceOf<List<UniqueIds>>(result, "The result should be a List<M_UniqueIds>");
        }

        [Test]
        public void ScanCode_ReturnsEmptyList_WhenVariablesDoNotExist()
        {
            // Arrange
            var ktgemvar = CreateKtgemvarWithVariables(
                new List<DataVariable>(),
                new List<EquipmentConstant>(),
                new List<DynamicVariable>(),
                new List<StatusVariable>()
            );

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.IsEmpty(result, "The result should be an empty list");
        }


        private Ktgem CreateKtgemvarWithVariables(List<DataVariable> dataVariables, List<EquipmentConstant> equipmentConstants, List<DynamicVariable> dynamicVariables, List<StatusVariable> statusVariables)
        {
            return new Ktgem
            {
                DataVariables = dataVariables,
                EquipmentConstants = equipmentConstants,
                DynamicVariables = dynamicVariables,
                StatusVariables = statusVariables
            };
        }

    }
}
