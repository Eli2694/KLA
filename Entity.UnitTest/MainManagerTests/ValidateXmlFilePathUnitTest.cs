using Entity;
using Entity.EntityInterfaces;
using Entity.Scanners;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using System;
using Utility_LOG;

namespace Entity.Tests
{
    [TestFixture]
    public class MainManagerTests
    {
        private Mock<AlarmScanner> _mockAlarmScanner;
        private Mock<EventScanner> _mockEventScanner;
        private Mock<VariableScanner> _mockVariableScanner;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<LogManager> _mockLog;
        private Mock<IFileSystem> _mockFileSystem;
        private MainManager _mainManager;

        [SetUp]
        public void SetUp()
        {
            _mockAlarmScanner = new Mock<AlarmScanner>();
            _mockEventScanner = new Mock<EventScanner>();
            _mockVariableScanner = new Mock<VariableScanner>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLog = new Mock<LogManager>();
            _mockFileSystem = new Mock<IFileSystem>();

            _mainManager = new MainManager(
                _mockAlarmScanner.Object,
                _mockEventScanner.Object,
                _mockVariableScanner.Object,
                _mockUnitOfWork.Object,
                _mockLog.Object,
                _mockFileSystem.Object);
        }

        [Test]
        public void ValidateXmlFilePath_FilePathExists_ReturnsTrue()
        {
            // Arrange
            var filePath = "test.xml";
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            var result = _mainManager.ValidateXmlFilePath(filePath);

            // Assert
            Assert.IsTrue(result);
            //_mockLog.Verify(log => log.LogError(It.IsAny<string>(), It.IsAny<LogProviderType>()), Times.Never);
        }

        [Test]
        public void ValidateXmlFilePath_FilePathDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var filePath = "test.xml";
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var result = _mainManager.ValidateXmlFilePath(filePath);

            // Assert
            Assert.IsFalse(result);
            //_mockLog.Verify(log => log.LogError(It.IsAny<string>(), It.IsAny<LogProviderType>()), Times.Once);
        }

        [Test]
        public void ValidateXmlFilePath_ErrorOccursDuringValidation_ReturnsFalse()
        {
            // Arrange
            var filePath = "test.xml";
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Throws<Exception>();

            // Act
            var result = _mainManager.ValidateXmlFilePath(filePath);

            // Assert
            Assert.IsFalse(result);
           // _mockLog.Verify(log => log.LogError(It.IsAny<string>(), It.IsAny<LogProviderType>()), Times.Once);
        }
    }
}
