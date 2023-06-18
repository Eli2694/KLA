//using Entity.Scanners;
//using Model;
//using Moq;
//using Repository.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Utility_LOG;

//namespace Entity.UnitTest
//{
//	[TestFixture]
//	public class MainManagerTests
//	{
//		private Mock<AlarmScanner> _mockAlarmScanner;
//		private Mock<EventScanner> _mockEventScanner;
//		private Mock<VariableScanner> _mockVariableScanner;
//		private Mock<IUnitOfWork> _mockUnitOfWork;
//		private Mock<LogManager> _mockLogManager;
//		private MainManager _mainManager;

//		[SetUp]
//		public void Setup()
//		{
//			_mockAlarmScanner = new Mock<AlarmScanner>(_mockLogManager);
//			_mockEventScanner = new Mock<EventScanner>(_mockLogManager);
//			_mockVariableScanner = new Mock<VariableScanner>(_mockLogManager);
//			_mockUnitOfWork = new Mock<IUnitOfWork>();
//			_mockLogManager = new Mock<LogManager>();
//			_mainManager = new MainManager(_mockAlarmScanner.Object, _mockEventScanner.Object, _mockVariableScanner.Object, _mockUnitOfWork.Object, _mockLogManager.Object);
//		}

//		//[Test]
//		//public void XmlToSeperatedScopes_WhenFilePathDoesNotExist_ReturnsNull()
//		//{
//		//	// Arrange
//		//	string filePath = "nonexistent.xml";
//		//	//_mockVariableScanner.Setup(scanner => scanner.ScanCode(It.IsAny<KlaXML>())).Returns(new List<UniqueIds>());
//		//	//_mockEventScanner.Setup(scanner => scanner.ScanCode(It.IsAny<KlaXML>())).Returns(new List<UniqueIds>());
//		//	//_mockAlarmScanner.Setup(scanner => scanner.ScanCode(It.IsAny<KlaXML>())).Returns(new List<UniqueIds>());

//		//	// Act
//		//	var result = _mainManager.XmlToSeperatedScopes(filePath);

//		//	// Assert
//		//	Assert.IsNull(result, "The result should be null when the file path does not exist");
//		//}

//		[Test]
//		public void XmlToSeperatedScopes_WhenFilePathExists_ReturnsSeperatedScopes()
//		{
//			// Arrange
//			string filePath = "existing.xml";
//			KlaXML klaXml = new KlaXML();
//			List<UniqueIds> variablesList = new List<UniqueIds>();
//			List<UniqueIds> eventsList = new List<UniqueIds>();
//			List<UniqueIds> alarmsList = new List<UniqueIds>();

//			_mockVariableScanner.Setup(scanner => scanner.ScanCode(klaXml)).Returns(variablesList);
//			_mockEventScanner.Setup(scanner => scanner.ScanCode(klaXml)).Returns(eventsList);
//			_mockAlarmScanner.Setup(scanner => scanner.ScanCode(klaXml)).Returns(alarmsList);

//			_mockVariableScanner.Setup(scanner => scanner.CompareXmlScopeWithDBScope(variablesList, It.IsAny<List<UniqueIds>>())).Returns(true);
//			_mockEventScanner.Setup(scanner => scanner.CompareXmlScopeWithDBScope(eventsList, It.IsAny<List<UniqueIds>>())).Returns(true);
//			_mockAlarmScanner.Setup(scanner => scanner.CompareXmlScopeWithDBScope(alarmsList, It.IsAny<List<UniqueIds>>())).Returns(true);

//			_mockLogManager.Setup(log => log.LogError(It.IsAny<string>(), LogProviderType.Console));
//			//_mockLogManager.Setup(log => log.LogError(It.IsAny<string>(), LogProviderType.File));

//			_mockUnitOfWork.Setup(uow => uow.UniqueIds.GetAll()).Returns(new List<UniqueIds>());

//			// Act
//			var result = _mainManager.XmlToSeperatedScopes(filePath);

//			// Assert
//			Assert.IsNotNull(result, "The result should not be null");
//			Assert.AreSame(variablesList, result.VariablesList, "The variables list should be the same");
//			Assert.AreSame(eventsList, result.EventsList, "The events list should be the same");
//			Assert.AreSame(alarmsList, result.AlarmsList, "The alarms list should be the same");

//			_mockVariableScanner.Verify(scanner => scanner.ScanCode(klaXml), Times.Once);
//			_mockEventScanner.Verify(scanner => scanner.ScanCode(klaXml), Times.Once);
//			_mockAlarmScanner.Verify(scanner => scanner.ScanCode(klaXml), Times.Once);
//			_mockLogManager.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.Console), Times.Never);
//			//_mockLogManager.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.File), Times.Never);
//		}

//		//[Test]
//		//public void XmlToSeperatedScopes_WhenExceptionOccurs_LogsAndThrowsException()
//		//{
//		//	// Arrange
//		//	string filePath = "existing.xml";
//		//	_mockVariableScanner.Setup(scanner => scanner.ScanCode(It.IsAny<KlaXML>())).Throws(new System.Exception("Some error occurred"));

//		//	// Act and Assert
//		//	Assert.Throws<System.Exception>(() => _mainManager.XmlToSeperatedScopes(filePath));

//		//	_mockLogManager.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.Console), Times.Once);
//		//	//_mockLogManager.Verify(log => log.LogError(It.IsAny<string>(), LogProviderType.File), Times.Once);
//		//}
//	}
//}
