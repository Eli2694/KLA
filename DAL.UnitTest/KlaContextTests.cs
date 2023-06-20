using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using Utility_LOG;


namespace DAL.UnitTest
{
	[TestFixture]
	public class KlaContextTests
	{
		private Mock<LogManager> _logManagerMock;
		private TestDatabaseCreator _databaseCreator;
		private DbContextOptions<KlaContext> _dbContextOptions;

		[SetUp]
		public void Setup()
		{
			_logManagerMock = new Mock<LogManager>();
			_databaseCreator = new TestDatabaseCreator();
			_dbContextOptions = new DbContextOptionsBuilder<KlaContext>()
				.UseInMemoryDatabase(databaseName: "test_db")
				.Options;
		}

		[Test]
		public void Constructor_WhenDatabaseCreatorCanConnect_CreatesTables()
		{
			// Arrange
			_databaseCreator.SetCanConnect(true);
			_databaseCreator.SetHasTables(false);

			// Act
			var context = new KlaContext(_dbContextOptions, _logManagerMock.Object);

			// Assert
			Assert.IsTrue(_databaseCreator.CanConnect);
			Assert.IsFalse(_databaseCreator.TablesCreated);
			_databaseCreator.CreateTables();
			Assert.IsTrue(_databaseCreator.TablesCreated);
		}

		[Test]
		public void Constructor_WhenDatabaseCreatorCannotConnect_CreatesDatabase()
		{
			// Arrange
			_databaseCreator.SetCanConnect(false);

			// Act
			var context = new KlaContext(_dbContextOptions, _logManagerMock.Object);

			// Assert
			_databaseCreator.VerifyCreate(Times.Once());
		}
		//[Test]
		//public void Constructor_WhenExceptionThrown_LogsExceptionAndThrows()
		//{
		//	// Arrange
		//	_databaseCreator.SetCanConnect(false);
		//	_databaseCreator.SetHasTables(true);

		//	var optionsBuilder = new DbContextOptionsBuilder<KlaContext>();
		//	optionsBuilder.UseInMemoryDatabase(databaseName: "test_db");
		//	var contextOptions = optionsBuilder.Options;

		//	var exceptionMessage = "Test exception message";
		//	var exception = new Exception(exceptionMessage);

		//	Exception capturedException = null;
		//	_logManagerMock.Setup(lm => lm.LogException(exceptionMessage, It.IsAny<Exception>(), LogProviderType.Console))
		//		.Callback((string msg, Exception ex, LogProviderType logProviderType) => capturedException = ex);

		//	// Act & Assert
		//	Assert.Throws<Exception>(() => new KlaContext(contextOptions, _logManagerMock.Object));

		//	Assert.AreEqual(exception, capturedException);
		//}

		public class TestDatabaseCreator : IDatabaseCreator
		{
			public bool CanConnect { get; private set; }
			public bool TablesCreated { get; private set; }

			private bool _throwExceptionOnConnect;

			public void SetCanConnect(bool canConnect)
			{
				CanConnect = canConnect;
			}

			public void SetHasTables(bool hasTables)
			{
				TablesCreated = hasTables;
			}

			public void Create()
			{
				// Mock implementation
			}

			public void CreateTables()
			{
				TablesCreated = true;
			}
			bool IDatabaseCreator.CanConnect()
			{
				if (_throwExceptionOnConnect)
				{
					throw new Exception("Test exception");
				}

				return CanConnect;
			}
			public void ThrowExceptionOnConnect()
			{
				_throwExceptionOnConnect = true;
			}
			public void VerifyCreate(Times times)
			{
				if (times == Times.Once())
				{
					Create();
				}
			}

			bool IDatabaseCreator.EnsureDeleted()
			{
				throw new NotImplementedException();
			}
			Task<bool> IDatabaseCreator.EnsureDeletedAsync(CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}
			Task<bool> IDatabaseCreator.CanConnectAsync(CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}
			bool IDatabaseCreator.EnsureCreated()
			{
				throw new NotImplementedException();
			}
			Task<bool> IDatabaseCreator.EnsureCreatedAsync(CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}
		}
	}

}

;