using Entity.Scanners;
using Model;
using Model.XmlModels;
using Moq;
using NUnit.Framework;
using Utility_LOG;

namespace Entity.UnitTest.ScannersTest
{
    [NonParallelizable]
    [TestFixture]
    public class AlarmScannerTest
    {
        //private Mock<LogManager> _mockLogManager;
        private AlarmScanner _target;

        [SetUp]
        public void Setup()
        {
            //_mockLogManager = new Mock<LogManager>();
            _target = new AlarmScanner();
        }


        [Test]
        [TestCase("Alarm 1", "alarm", (uint)1)]
        [TestCase("Alarm 2", "alarm", (uint)2)]
        public void ScanCode_MapsAlarmDataCorrectly_WhenAlarmsExist(string alarmName, string scope, uint alarmId)
        {
            // Arrange
            var alarms = new List<Alarm>
            {
                new Alarm { Id = alarmId, Name = alarmName }
            };
            var ktgemvar = CreateKtgemvarWithAlarms(alarms);

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);


            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count, "There should be one mapped M_UniqueIds object");

                UniqueIds uniqueId = result.FirstOrDefault();
                Assert.IsNotNull(uniqueId, "Mapped M_UniqueIds object should not be null");
                Assert.AreEqual("Alarm", uniqueId.EntityType, "EntityType should be set to 'Alarm'");
                Assert.AreEqual(alarmName, uniqueId.Name, "Name should be mapped correctly");
                Assert.AreEqual(scope, uniqueId.Scope, "Scope should be set to 'alarm'");
            });
        }

        [Test]
        public void ScanCode_ReturnsListOfMUniqueIds_WhenAlarmsExist()
        {
            // Arrange
            var alarms = new List<Alarm>
            {
                new Alarm { Id = 1, Name = "Alarm 1" },
                new Alarm { Id = 2, Name = "Alarm 2" },
                new Alarm { Id = 3, Name = "Alarm 3" }
            };
            var ktgemvar = CreateKtgemvarWithAlarms(alarms);

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<List<UniqueIds>>(result, "The result should be a List<M_UniqueIds>");
                Assert.AreEqual(alarms.Count, result.Count, "The number of generated M_UniqueIds objects should be equal to the number of alarms");
            });
        }

        [Test]
        public void ScanCode_ReturnsEmptyList_WhenNoAlarmsExist()
        {
            // Arrange
            var ktgemvar = CreateKtgemvarWithAlarms(new List<Alarm>());

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.IsEmpty(result, "The result should be an empty list");
        }

        private Ktgem CreateKtgemvarWithAlarms(List<Alarm> alarms)
        {
            return new Ktgem { Alarms = alarms };
        }
    }
}