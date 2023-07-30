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
    public class EventScannerTests
    {
        //private Mock<LogManager> _mockLogManager;
        private EventScanner _target;

        [SetUp]
        public void Setup()
        {
            //_mockLogManager = new Mock<LogManager>();
            _target = new EventScanner();
        }

        [Test]
        public void ScanCode_MapsEventDataCorrectly_WhenEventsExist()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" },
                new Event { Id = 3, Name = "Event 3" }
            };
            var ktgemvar = CreateKtgemvarWithEvents(events);

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.Multiple(() =>
            {
                foreach (var evnt in events)
                {
                    UniqueIds uniqueId = result.Find(u => u.ID == evnt.Id.ToString());

                    Assert.IsNotNull(uniqueId, $"Mapped M_UniqueIds object for Event {evnt.Id} should not be null");
                    Assert.AreEqual("Event", uniqueId.EntityType, $"EntityType for Event {evnt.Id} should be set to 'Event'");
                    Assert.AreEqual(evnt.Name, uniqueId.Name, $"Name for Event {evnt.Id} should be mapped correctly");
                    Assert.AreEqual("event", uniqueId.Scope, $"Scope for Event {evnt.Id} should be set to 'event'");
                }
            });
        }

        [Test]
        public void ScanCode_ReturnsListOfMUniqueIds_WhenEventsExist()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" },
                new Event { Id = 3, Name = "Event 3" }
            };
            var ktgemvar = CreateKtgemvarWithEvents(events);

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.IsInstanceOf<List<UniqueIds>>(result, "The result should be a List<M_UniqueIds>");
        }

        [Test]
        public void ScanCode_ReturnsEmptyList_WhenEventsDoNotExist()
        {
            // Arrange
            var ktgemvar = CreateKtgemvarWithEvents(new List<Event>());

            // Act
            List<UniqueIds> result = _target.ScanKtgemContent(ktgemvar);

            // Assert
            Assert.IsEmpty(result, "The result should be an empty list");
        }

        private Ktgem CreateKtgemvarWithEvents(List<Event> events)
        {
            return new Ktgem { Events = events };
        }
    }
}
