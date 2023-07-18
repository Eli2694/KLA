using Entity.EntityInterfaces;
using Model;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.UnitTest.MainManagerTests
{
	[TestFixture]
	public class PrepareAliasIfNotExistingUnitTest
	{
		private Mock<IFileSystem> _mockFileSystem;
		[Test]
		public void PrepareAliasIfNotExisting_AliasDoesNotExist_ReturnsNewAlias()
		{
			// Arrange
			var uniqueId = new UniqueIds
			{
				ID = "1",
				Name = "UniqueId1",
				Scope = "scope1"
			};
			var aliasName = "Alias1";
			var existingAliases = new HashSet<string>();

			var mockLog = new Mock<LogManager>();
			var mockAliasesRepository = new Mock<IAliasesRepository>();
			var mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockFileSystem = new Mock<IFileSystem>();
			mockUnitOfWork.Setup(uow => uow.Aliases).Returns(mockAliasesRepository.Object);

			var mainManager = new MainManager(null, null, null, mockUnitOfWork.Object, mockLog.Object, _mockFileSystem.Object);

			// Act
			var result = mainManager.PrepareAliasIfNotExisting(uniqueId.ID, uniqueId.Name, uniqueId.Scope, aliasName, existingAliases);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(uniqueId.ID, result.ID);
			Assert.AreEqual(uniqueId.Name, result.PreviousAliasName);
			Assert.AreEqual(aliasName, result.CurrentAliasName);
			Assert.AreEqual(uniqueId.Scope, result.Scope);
			mockAliasesRepository.Verify(repo => repo.AddRange(It.IsAny<List<Aliases>>()), Times.Never);
			mockUnitOfWork.Verify(uow => uow.Complete(), Times.Never);
		}

		[Test]
		public void PrepareAliasIfNotExisting_AliasExists_ReturnsNullAndLogsWarning()
		{
			// Arrange
			var uniqueId = new UniqueIds
			{
				ID = "1",
				Name = "UniqueId1",
				Scope = "scope1"
			};
			var aliasName = "Alias1";
			var existingAliases = new HashSet<string> { "Alias1" };

			var mockLog = new Mock<LogManager>();
			var mockAliasesRepository = new Mock<IAliasesRepository>();
			var mockUnitOfWork = new Mock<IUnitOfWork>();
			mockUnitOfWork.Setup(uow => uow.Aliases).Returns(mockAliasesRepository.Object);

			var mainManager = new MainManager(null, null, null, mockUnitOfWork.Object, mockLog.Object, _mockFileSystem.Object);

			// Act
			var result = mainManager.PrepareAliasIfNotExisting(uniqueId.ID, uniqueId.Name, uniqueId.Scope, aliasName, existingAliases);

			// Assert
			Assert.IsNull(result);
			mockAliasesRepository.Verify(repo => repo.AddRange(It.IsAny<List<Aliases>>()), Times.Never);
			mockUnitOfWork.Verify(uow => uow.Complete(), Times.Never);
		}
	}
}
