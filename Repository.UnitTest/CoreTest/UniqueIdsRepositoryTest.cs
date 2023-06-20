using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using Repository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Repository.UnitTest.CoreTest
{
	[TestFixture]
	public class UniqueIdsRepositoryTests
	{
		private UniqueIdsRepository _uniqueIdsRepository;
		private KlaContext _context;
		private Mock<LogManager> _logManagerMock;

		[SetUp]
		public void Setup()
		{
			_logManagerMock = new Mock<LogManager>();
			var options = new DbContextOptionsBuilder<KlaContext>()
				.UseInMemoryDatabase(databaseName: "test_db")
				.Options;

			_context = new KlaContext(options, _logManagerMock.Object);
			_uniqueIdsRepository = new UniqueIdsRepository(_context, _logManagerMock.Object);
		}


		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		private void AddTestData(IEnumerable<UniqueIds> uniqueIds)
		{
			_context.Unique_Ids.AddRange(uniqueIds);
			_context.SaveChanges();
		}

		[Test]
		public void GetSpecificScope_WhenScopeExists_ReturnsMatchingUniqueIds()
		{
			// Arrange
			var scope = "test";
			var uniqueIds = new List<UniqueIds>
		{
			new UniqueIds { ID = "1", Scope = "test", Name = "yoyo", EntityType = "alarm" },
			new UniqueIds { ID = "2", Scope = "other", Name = "yoyo", EntityType = "alarm" },
			new UniqueIds { ID = "3", Scope = "test", Name = "yoyo", EntityType = "alarm" }
		};
			AddTestData(uniqueIds);

			// Act
			var result = _uniqueIdsRepository.GetSpecificScope(scope);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.All(x => x.Scope == scope));
			Assert.That(result.All(x => x.Name == "yoyo")); 
			Assert.That(result.All(x => x.EntityType == "alarm"));
		}

		[Test]
		public void GetSpecificScope_WhenScopeDoesNotExist_ReturnsEmptyList()
		{
			// Arrange
			var scope = "nonexistent";
			var uniqueIds = new List<UniqueIds>
		{
			new UniqueIds { ID = "1", Scope = "test", Name = "yoyo", EntityType = "alarm" },
			new UniqueIds { ID = "2", Scope = "other", Name = "yoyo", EntityType = "alarm" },
			new UniqueIds { ID = "3", Scope = "test", Name = "yoyo", EntityType = "alarm" }
		};
			AddTestData(uniqueIds);

			// Act
			var result = _uniqueIdsRepository.GetSpecificScope(scope);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Empty);
		}
	}
}
