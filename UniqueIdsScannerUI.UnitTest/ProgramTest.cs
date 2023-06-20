using Entity;
using Entity.Scanners;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework.Interfaces;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using UniqueIdsScannerUI;
using Microsoft.Extensions.Hosting;
using DAL;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.InMemory;


namespace UniqueIdsScannerUI.UnitTest
{
	[TestFixture]
	public class ProgramTest
	{
		private Mock<IConfiguration> _configurationMock;
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IUniqueIdsRepository> _uniqueIdsRepositoryMock;
		private Mock<IUserRepository> _userRepositoryMock;
		private Mock<LogManager> _logManagerMock;

		[SetUp]
		public void Setup()
		{
			_configurationMock = new Mock<IConfiguration>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_uniqueIdsRepositoryMock = new Mock<IUniqueIdsRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_logManagerMock = new Mock<LogManager>();
		}

		[Test]
		public void CreateHostBuilder_ValidArgs_ReturnsHostBuilder()
		{
			// Arrange
			var connectionString = "MyConnectionString";

			_configurationMock.Setup(c => c["ConnectionString:Value"]).Returns(connectionString);

			var services = new ServiceCollection();
			services.AddSingleton<App>();
			services.AddSingleton(_configurationMock.Object);
			services.AddSingleton(_logManagerMock.Object);
			services.AddTransient<IUnitOfWork>(_ => _unitOfWorkMock.Object);
			services.AddTransient<IUniqueIdsRepository>(_ => _uniqueIdsRepositoryMock.Object);
			services.AddTransient<IUserRepository>(_ => _userRepositoryMock.Object);
			services.AddTransient<AlarmScanner>();
			services.AddTransient<EventScanner>();
			services.AddTransient<VariableScanner>();
			services.AddSingleton<MainManager>();

			// Act
			var hostBuilder = CreateHostBuilder(null);
			var host = hostBuilder.Build();
			var serviceProvider = host.Services;
			var app = serviceProvider.GetRequiredService<App>();

			// Assert
			Assert.That(hostBuilder, Is.Not.Null);
			Assert.That(app, Is.Not.Null);
		}

		private IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<App>();
					services.AddSingleton(_configurationMock.Object);
					services.AddDbContext<KlaContext>(options =>
						options.UseInMemoryDatabase(databaseName: "test_db")
							.UseLoggerFactory(LoggerFactory.Create(builder =>
							{
								builder.AddFilter((category, level) =>
									!category.Equals("Microsoft.EntityFrameworkCore.Database.Command") ||
									level == LogLevel.Error);
							}))
					);
					services.AddSingleton(_logManagerMock.Object);
					services.AddTransient<IUnitOfWork>(_ => _unitOfWorkMock.Object);
					services.AddTransient<IUniqueIdsRepository>(_ => _uniqueIdsRepositoryMock.Object);
					services.AddTransient<IUserRepository>(_ => _userRepositoryMock.Object);
					services.AddTransient<AlarmScanner>();
					services.AddTransient<EventScanner>();
					services.AddTransient<VariableScanner>();
					services.AddSingleton<MainManager>();
				});
		}
	}
}

