using CommandLine.Text;
using CommandLine;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Utility_LOG;
using static Utility_LOG.LogManager;
using UniqueIdsScannerUI;
using Model;
using Entity;
using Microsoft.Extensions.Configuration;

public class App
{
	private readonly LogManager _log;
    private readonly MainManager _mainManager;
    private readonly IConfiguration _settings;

    public App(LogManager log, MainManager mainManager, IConfiguration settings)
    {
		_log = log;
        _mainManager = mainManager;
        _settings = settings;
    }
    internal void Run(string[] args)
    {

        //args = new string[3];
        //args[0] = "--update"; args[1] = "-f"; args[2] = @"C:\ZionNet\DevOps\KLA\DataDictionaryValidation-ExampleApp\InputFiles\ATLAS.reassign.xml"; // Uniqe.exe --update -f/-c/-a path = verify&update else = verify
        ////args[0] = "-f"; args[1] = "path";

        //if user acces without args params then start user interface
        if (args.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Instructions:");
            Console.WriteLine("For Updating The Database Use: UniqueIdsScanner.exe --update");
            Console.WriteLine("For Verifying The Content Of Xml Files Use: UniqueIdsScanner.exe --verify");
            Console.ResetColor();
            
        }
        else
        {

            var listOfXmlFileFromAppSettings = _settings.GetSection("XmlFilesPath").Get<List<string>>();
          
            if(_mainManager.ValidateXmlFilePaths(listOfXmlFileFromAppSettings))
            {
                using (var parser = new CommandLine.Parser((settings) =>
                {
                    settings.CaseSensitive = true;
                }))
                {
                    //add logs
                    var result = Parser.Default.ParseArguments<CliOptions>(args)
                   .WithParsed(Options => {
                       if (string.IsNullOrWhiteSpace(Options.filePath))
                       {
                           Console.WriteLine("Invalid command line arguments");
                           throw new ArgumentException("filePath is null or empty or white-space");
                       }
                       else
                       {
                           M_SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(Options.filePath);
                           M_SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());

                           Console.WriteLine("verifying: " + Options.filePath.Trim());
                           //verifying
                           if (xmlScopes != null)
                           {
                               // go through all the dictionaries and compare their values with db
                               if (_mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes))
                               {
                                   Console.WriteLine("no conflicts");

                                   //if user selected the --update cliCommand option
                                   if (Options.isUpdate)
                                   {
                                       _mainManager.UpdateDatabaseWithNewUniqueIds();
                                   }
                               }
                           }
                           else
                           {
                               _log.LogError("Can't read from xml file", LogProviderType.Console);
                               throw new Exception();
                           }
                       }
                   })
                   //if user enterd wrong arguments
                   .WithNotParsed((errs) => throw new ArgumentException($"Failed to parse command line arguments: {errs}"));
                }
            }
        }
    }
}

