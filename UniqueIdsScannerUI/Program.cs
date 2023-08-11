using DAL;
using Entity;
using Entity.EntityInterfaces;
using Entity.Scanners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository.Core;
using Repository.Interfaces;
using Utility_LOG;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var logManager = services.GetRequiredService<LogManager>();


try
{
    var app = services.GetRequiredService<App>();
    app.Run(args);
	logManager.LogInfo("The app has finished running", LogProviderType.Console);
}
catch (Exception)
{
    logManager.LogError("Application Error: Check Log File",LogProviderType.Console);
}


static IHostBuilder CreateHostBuilder(string[] args)
{

	var config = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("UniqeIdsScanner_ENVIRONMENT") ?? "Production"}.json", optional: true)
		.AddEnvironmentVariables()
		.Build();

	var connectionString = GetConnectionString(config); 
	
	return Host.CreateDefaultBuilder(args)
	 .ConfigureServices((hostContext, services) =>
	 {

         services.AddSingleton<App>();
		 services.AddDbContext<KlaContext>(options =>
			 options.UseSqlServer(connectionString)
			 .UseLoggerFactory(LoggerFactory.Create(builder =>
			 {
				 builder.AddFilter((category, level) =>
					 !category.Equals("Microsoft.EntityFrameworkCore.Database.Command") || level == LogLevel.Error);
			 }))
         );
		 services.AddSingleton<LogManager>();
		 services.AddTransient<IUnitOfWork, UnitOfWork>();
		 services.AddTransient<IUniqueIdsRepository, UniqueIdsRepository>();
		 services.AddTransient<IUserRepository, UserRepository>();
		 services.AddTransient<IFileSystem, RealFileSystem>();
		 services.AddTransient<AlarmScanner>();
		 services.AddTransient<EventScanner>();
		 services.AddTransient<VariableScanner>();
		 services.AddSingleton<MainManager>();
	 });
}

static string GetConnectionString(IConfiguration config)
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

    if (dbHost == null || dbName == null || dbPassword == null)
    {
        // If any of the environment variables were not found, return the connection string from appsettings
        return config["ConnectionString:Value"];
    }

    return $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa; Password={dbPassword};TrustServerCertificate=true";
}
