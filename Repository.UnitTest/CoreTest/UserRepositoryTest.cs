using DAL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using Repository.Core;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Repository.UnitTest.CoreTest
{
	[TestFixture]
	public class UserRepositoryTests
	{
		private UserRepository _userRepository;
		private KlaContext _context;
		private Mock<LogManager> _logManagerMock;

		[SetUp]
		public void Setup()
		{
			_logManagerMock = new Mock<LogManager>();

			// Create an in-memory database connection
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			// Create a new KlaContext using the in-memory database connection
			var options = new DbContextOptionsBuilder<KlaContext>()
				.UseSqlite(connection)
				.Options;
			_context = new KlaContext(options, _logManagerMock.Object);

			// Ensure the in-memory database is created and the schema is applied
			_context.Database.EnsureCreated();

			// Seed the in-memory database with test data
			_context.Users.AddRange(
				new List<User>
				{
					new User { UserID = "1", Password = "12345" },
					new User { UserID = "2", Password = "1234" },
					new User { UserID = "3", Password = "123yoyo" }
				});
			_context.SaveChanges();

			// Create an instance of UserRepository with the in-memory context
			_userRepository = new UserRepository(_context, null);
		}

		[Test]
		public void GetValidatedUser_WhenUserExists_ReturnsUser()
		{
			// Arrange
			var userId = "1";

			// Act
			var result = _userRepository.GetValidatedUser(userId);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(userId, result.UserID);
		}

		[Test]
		public void GetValidatedUser_WhenUserDoesNotExist_ReturnsNull()
		{
			// Arrange
			var userId = "999";

			// Act
			var result = _userRepository.GetValidatedUser(userId);

			// Assert
			Assert.IsNull(result);
		}

		[TearDown]
		public void Cleanup()
		{
			// Dispose the in-memory database connection and context
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}
	}
}
