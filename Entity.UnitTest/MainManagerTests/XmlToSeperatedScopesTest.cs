//using Entity.EntityInterfaces;
//using Entity.Scanners;
//using Model;
//using Moq;
//using NUnit.Framework;
//using Repository.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;
//using Utility_LOG;

//namespace Entity.UnitTest.MainManagerTests
//{
//    [TestFixture]
//    public class XmlToSeperatedScopesTest
//    {
//        private Mock<AlarmScanner> _mockAlarmScanner;
//        private Mock<EventScanner> _mockEventScanner;
//        private Mock<VariableScanner> _mockVariableScanner;
//        private Mock<IUnitOfWork> _mockUnitOfWork;
//        private Mock<LogManager> _mockLog;
//        private Mock<IFileSystem> _mockFileSystem;
//        private MainManager _mainManager;
//        private string _xmlFilePath;

//        [SetUp]
//        public void Setup()
//        {
//            _mockAlarmScanner = new Mock<AlarmScanner>();
//            _mockEventScanner = new Mock<EventScanner>();
//            _mockVariableScanner = new Mock<VariableScanner>();
//            _mockUnitOfWork = new Mock<IUnitOfWork>();
//            _mockLog = new Mock<LogManager>();
//            _mockFileSystem = new Mock<IFileSystem>();
//            _xmlFilePath = @"C:\Path\To\TestFile.xml";

//            _mainManager = new MainManager(
//                _mockAlarmScanner.Object,
//                _mockEventScanner.Object,
//                _mockVariableScanner.Object,
//                _mockUnitOfWork.Object,
//                _mockLog.Object,
//                _mockFileSystem.Object
//            );
//        }

//        [Test]
//        public void XmlToSeperatedScopes_FileDoesNotExist_ReturnsNull()
//        {
//            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

//            var result = _mainManager.XmlToSeperatedScopes(_xmlFilePath);

//            Assert.IsNull(result);
//            //_mockLog.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.Console), Times.Once);
//        }

//        [Test]
//        public void XmlToSeperatedScopes_FileExistsButNotXml_ReturnsNull()
//        {
//            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
//            _mockFileSystem.Setup(fs => fs.GetFileExtension(It.IsAny<string>())).Returns(".txt");

//            var result = _mainManager.XmlToSeperatedScopes(_xmlFilePath);

//            Assert.IsNull(result);
//            //_mockLog.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.Console), Times.Once);
//        }


//    }
//}
