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
}
catch (Exception ex)
{
  Console.WriteLine(ex.ToString());
}


static IHostBuilder CreateHostBuilder(string[] args)
{

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("UniqeIdsScanner_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    // var connectionString = GetConnectionString(); 
    var connectionString = config["ConnectionString:Value"];


    return Host.CreateDefaultBuilder(args)
     .ConfigureServices((hostContext, services) =>
     {
         services.AddSingleton<App>();
         services.AddDbContext<KlaContext>(options =>
             options.UseSqlServer(connectionString)
             .UseLoggerFactory(LoggerFactory.Create(builder =>
             {
                 builder.AddFilter((category, level) =>
                     category != null &&
                     !category.Equals("Microsoft.EntityFrameworkCore.Database.Command") || level == LogLevel.Error);
             }))
         );
         services.AddSingleton<LogManager>();
         services.AddTransient<IUnitOfWork, UnitOfWork>();
         services.AddTransient<IUniqueIdsRepository, UniqueIdsRepository>();
         services.AddTransient<IUserRepository, UserRepository>();
         services.AddTransient<AlarmScanner>();
         services.AddTransient<EventScanner>();
         services.AddTransient<VariableScanner>();
         services.AddSingleton<MainManager>();
     });


}

//static string GetConnectionString()
//{
//    var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
//                 ?? throw new ArgumentNullException("DB_HOST environment variable not found");
//    var dbName = Environment.GetEnvironmentVariable("DB_NAME")
//                 ?? throw new ArgumentNullException("DB_NAME environment variable not found");
//    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD")
//                 ?? throw new ArgumentNullException("DB_SA_PASSWORD environment variable not found");


//    return $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa; Password={dbPassword};TrustServerCertificate=true";
//}
