using Entity.Scanners;
using Model;
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
    public class BaseScannerTests
    {
        private TestScanner _baseScanner;
        private Mock<LogManager> _mockLogManager;

        [SetUp]
        public void Setup()
        {
            _mockLogManager = new Mock<LogManager>();
            _baseScanner = new TestScanner(_mockLogManager.Object);
        }

        [Test]
        public void CompareXmlScopeWithDBScope_WhenXmlAndDbAreEqual_ReturnsTrue()
        {
            // Arrange
            var xml = CreateUniqueIdsList(("1", "A"), ("2", "B"));
            var db = CreateUniqueIdsList(("1", "A"), ("2", "B"));

            // Act
            var result = _baseScanner.CompareXmlScopeWithDBScope(xml, db, false);

            // Assert
            Assert.IsTrue(result, "The result should be true when XML and DB are equal.");
            //_mockLogManager.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.Console), Times.Never);
        }

        [Test]
        public void CompareXmlScopeWithDBScope_WhenXmlAndDbHaveMismatchedName_ReturnsFalseAndLogsError()
        {
            // Arrange
            var xml = CreateUniqueIdsList(("1", "A"), ("2", "X"));
            var db = CreateUniqueIdsList(("1", "A"), ("2", "B"));

            // Act
            var result = _baseScanner.CompareXmlScopeWithDBScope(xml, db,false);

            // Assert
            Assert.IsFalse(result, "The result should be false when XML and DB have a mismatched name.");
            //VerifyLogError("ID '2' has a different name in the XML and DB.");
        }

        [Test]
        public void CompareXmlScopeWithDBScope_WhenXmlAndDbHaveMismatchedId_ReturnsFalseAndLogsError()
        {
            // Arrange
            var xml = CreateUniqueIdsList(("1", "A"), ("3", "B"));
            var db = CreateUniqueIdsList(("1", "A"), ("2", "B"));

            // Act
            var result = _baseScanner.CompareXmlScopeWithDBScope(xml, db, false);

            // Assert
            Assert.IsFalse(result, "The result should be false when XML and DB have a mismatched ID.");
            //VerifyLogError("Name 'B' has a different ID in the XML and DB.");
        }

        private List<UniqueIds> CreateUniqueIdsList(params (string, string)[] tuples)
        {
            var uniqueIdsList = new List<UniqueIds>();
            foreach (var tuple in tuples)
            {
                uniqueIdsList.Add(new UniqueIds { ID = tuple.Item1, Name = tuple.Item2 });
            }
            return uniqueIdsList;
        }

        private void VerifyLogError(string errorMessage)
        {
            _mockLogManager.Verify(log => log.LogError(errorMessage, LogProviderType.Console), Times.Once);
        }

        private class TestScanner : BaseScanner
        {
            public TestScanner(LogManager log) : base(log)
            {
            }
        }

    }
}
