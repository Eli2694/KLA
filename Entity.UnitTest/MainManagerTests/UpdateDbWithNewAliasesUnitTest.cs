using Entity.EntityInterfaces;
using Model;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.UnitTest.MainManagerTests
{
	[TestFixture]
	public class UpdateDbWithNewAliasesUnitTest
	{

		private Mock<IFileSystem> _mockFileSystem;
		[Test]
		public void UpdateDbWithNewAliases_AddAliasesToUnitOfWork_Success()
		{
			// Arrange
			var newAliases = new List<Aliases>
			{
				new Aliases { ID = "1", AliasPreviousName = "UniqueId1", AliasCurrentName = "Alias1", Scope = "scope1", AliasCreated = new System.DateTime(2023, 1, 1) },
				new Aliases { ID = "2", AliasPreviousName = "UniqueId2", AliasCurrentName = "Alias2", Scope = "scope2", AliasCreated = new System.DateTime(2023, 1, 1) },
				new Aliases { ID = "3", AliasPreviousName = "UniqueId3", AliasCurrentName = "Alias3", Scope = "scope3", AliasCreated = new System.DateTime(2023, 1, 1) }
			};

			var mockAliasesRepository = new Mock<IAliasesRepository>();
			var mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockFileSystem = new Mock<IFileSystem>();
			mockUnitOfWork.Setup(uow => uow.Aliases).Returns(mockAliasesRepository.Object);

			var mainManager = new MainManager(null, null, null, mockUnitOfWork.Object, null, _mockFileSystem.Object);

			// Act
			mainManager.UpdateDbWithNewAliases(newAliases);

			// Assert
			mockAliasesRepository.Verify(repo => repo.AddRange(newAliases), Times.Once);
			mockUnitOfWork.Verify(uow => uow.Complete(), Times.Once);
		}
	}
}
