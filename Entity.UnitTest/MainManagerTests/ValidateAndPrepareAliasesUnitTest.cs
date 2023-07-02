using Entity.EntityInterfaces;
using Entity.Scanners;
using Model;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.UnitTest.MainManagerTests
{
	[TestFixture]
	public class ValidateAndPrepareAliasesUnitTest
	{
		private MainManager _mainManager;
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IAliasesRepository> _aliasesRepositoryMock;
		private Mock<IUniqueIdsRepository> _uniqueIdsRepositoryMock;
		private Mock<LogManager> _logManagerMock;
		private Mock<AlarmScanner> _alarmScannerMock;
		private Mock<EventScanner> _eventScannerMock;
		private Mock<VariableScanner> _variableScannerMock;
		private Mock<IFileSystem> _mockFileSystem;

		[SetUp]
		public void Setup()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_aliasesRepositoryMock = new Mock<IAliasesRepository>();
			_uniqueIdsRepositoryMock = new Mock<IUniqueIdsRepository>();
			_logManagerMock = new Mock<LogManager>();
			_alarmScannerMock = new Mock<AlarmScanner>();
			_eventScannerMock = new Mock<EventScanner>();
			_variableScannerMock = new Mock<VariableScanner>();
			_mockFileSystem = new Mock<IFileSystem>();

			_unitOfWorkMock.SetupGet(uow => uow.Aliases).Returns(_aliasesRepositoryMock.Object);
			_unitOfWorkMock.SetupGet(uow => uow.UniqueIds).Returns(_uniqueIdsRepositoryMock.Object);

			_mainManager = new MainManager(_alarmScannerMock.Object,_eventScannerMock.Object,_variableScannerMock.Object,_unitOfWorkMock.Object,_logManagerMock.Object, _mockFileSystem.Object);
		}
		[Test]
		public void ValidateAndPrepareAliases_KeysNotFound_ThrowsKeyNotFoundException()
		{
			// Arrange
			var renameInfo = new Dictionary<string, string>
			{
				{ "Key1", "Alias1" },
				{ "Key2", "Alias2" }
			};

			_uniqueIdsRepositoryMock.Setup(repo => repo.GetAll())
				.Returns(new List<UniqueIds>());

			// Act & Assert
			Assert.Throws<KeyNotFoundException>(() => _mainManager.ValidateAndPrepareAliases(renameInfo));
		}

		[Test]
		public void ValidateAndPrepareAliases_AliasesNotExistInDb_AddsNewAliasesToDatabase()
		{
			// Arrange
			var renameInfo = new Dictionary<string, string>
			{
				{ "Key1", "Alias1" },
				{ "Key2", "Alias2" }
			};

			var existingAliases = new List<Aliases>();
			_aliasesRepositoryMock.Setup(repo => repo.GetAll())
				.Returns(existingAliases);

			_unitOfWorkMock.Setup(uow => uow.Aliases)
				.Returns(_aliasesRepositoryMock.Object);

			var uniqueId1 = new UniqueIds { ID = "1", Name = "Key1", Scope = "event" };
			var uniqueId2 = new UniqueIds { ID = "2", Name = "Key2", Scope = "alarm" };

			var uniqueIdsList = new List<UniqueIds> { uniqueId1, uniqueId2 };
			_uniqueIdsRepositoryMock.Setup(repo => repo.GetAll())
				.Returns(uniqueIdsList);

			_uniqueIdsRepositoryMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<UniqueIds, bool>>>()))
				.Returns<Expression<Func<UniqueIds, bool>>>(predicate =>
				{
					var compiledPredicate = predicate.Compile();
					return uniqueIdsList.Where(compiledPredicate).ToList();
				});

			_unitOfWorkMock.Setup(uow => uow.UniqueIds)
				.Returns(_uniqueIdsRepositoryMock.Object);


			_aliasesRepositoryMock.Setup(repo => repo.AddRange(It.IsAny<IEnumerable<Aliases>>()));

			// Act
			_mainManager.ValidateAndPrepareAliases(renameInfo);

			// Assert
			_aliasesRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<IEnumerable<Aliases>>()), Times.Once);
		}

	}
}
