using Model;
using Moq;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.UnitTest.MainManagerTests
{
	[TestFixture]
	public class UpdateDbWithNewAliasesUnitTest
	{
		[Test]
		public void UpdateDbWithNewAliases_AddAliasesToUnitOfWork_Success()
		{
			// Arrange
			var newAliases = new List<Aliases>
			{
				new Aliases { ID = "1", OriginalName = "UniqueId1", AliasName = "Alias1", UniqueIdScope = "scope1", AliasCreated = new System.DateTime(2023, 1, 1) },
				new Aliases { ID = "2", OriginalName = "UniqueId2", AliasName = "Alias2", UniqueIdScope = "scope2", AliasCreated = new System.DateTime(2023, 1, 1) },
				new Aliases { ID = "3", OriginalName = "UniqueId3", AliasName = "Alias3", UniqueIdScope = "scope3", AliasCreated = new System.DateTime(2023, 1, 1) }
			};

			var mockAliasesRepository = new Mock<IAliasesRepository>();
			var mockUnitOfWork = new Mock<IUnitOfWork>();
			mockUnitOfWork.Setup(uow => uow.Aliases).Returns(mockAliasesRepository.Object);

			var mainManager = new MainManager(null, null, null, mockUnitOfWork.Object, null);

			// Act
			mainManager.UpdateDbWithNewAliases(newAliases);

			// Assert
			mockAliasesRepository.Verify(repo => repo.AddRange(newAliases), Times.Once);
			mockUnitOfWork.Verify(uow => uow.Complete(), Times.Once);
		}
	}
}
