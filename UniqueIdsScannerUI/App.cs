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
using System.Linq;

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
        // Uncomment the following lines for testing purposes
        //args = new string[3];
        //args[0] = "--update";
        //args[1] = "-f";
        //args[2] = @"C:\ZionNet\DevOps\KLA\DataDictionaryValidation-ExampleApp\InputFiles\ATLAS.reassign.xml";

        // Uncomment the following lines for testing purposes
       // args = new string[2];
       // args[0] = "update"; args[1] = "-f";

        if (args.Length == 0)
        {
            ShowUserInterface();
        }
        else
        {
            ParseCommandLineArguments(args);
        }
    }

    private void ShowUserInterface()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Instructions:");
        Console.WriteLine("Xml Paths Reside in Configuration file:");
        Console.WriteLine("For Verifying The Content Of Xml Files Use: UniqueIdsScanner.exe --verify");
        Console.WriteLine("For Updating The Database Use : UniqueIdsScanner.exe --update");
        Console.WriteLine("Input Xml Path:");
        Console.ResetColor();
    }

    private void ParseCommandLineArguments(string[] args)
    {
        using (var parser = new CommandLine.Parser((settings) =>
        {
            settings.CaseSensitive = true;
        }))
        {
            var result = parser.ParseArguments<CliOptions>(args);

            result.WithParsed(ProcessCommandLineOptions)
                  .WithNotParsed(HandleCommandLineErrors);
        }
    }

    private void ProcessCommandLineOptions(CliOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.filePath))
        {
            VerifyXmlFile(options.filePath, options.isUpdate);
        }
        else
        {
            ProcessXmlFilesFromAppSettings(options.isUpdate);
        }
    }

    private void HandleCommandLineErrors(IEnumerable<Error> errors)
    {
        throw new ArgumentException($"Failed to parse command line arguments: {string.Join(", ", errors)}");
    }

    private void ProcessXmlFilesFromAppSettings(bool isUpdate)
    {
        List<string>? xmlFilesFromAppSettings = _settings.GetSection("XmlFilesPath").Get<List<string>>();

        if (xmlFilesFromAppSettings != null && _mainManager.ValidateXmlFilePaths(xmlFilesFromAppSettings))
        {
            foreach (string filePath in xmlFilesFromAppSettings)
            {
                VerifyXmlFile(filePath, isUpdate);
            }
        }
    }

    public void VerifyXmlFile(string filepath, bool isUpdate)
    {
        try
        {
            SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);
            SeperatedScopes? dbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());

            _log.LogEvent($"Verifying: {filepath.Trim()}", LogProviderType.Console);

            if (xmlScopes != null && _mainManager.CompareXmlScopesWithDBScopes(xmlScopes, dbScopes))
            {
                _log.LogEvent("No errors were found in the comparison between XML and DB ", LogProviderType.Console);

                if (isUpdate)
                {
                    _mainManager.UpdateDatabaseWithNewUniqueIds();
                }
            }
        }
        catch (Exception)
        {
            _log.LogError("Can't separate xml file to scopes", LogProviderType.Console);
        }    
    }
}




//using CommandLine.Text;
//using CommandLine;
//using Microsoft.Identity.Client;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.Reflection.Metadata;
//using System.Security.Cryptography;
//using Utility_LOG;
//using static Utility_LOG.LogManager;
//using UniqueIdsScannerUI;
//using Model;
//using Entity;
//using Microsoft.Extensions.Configuration;
//using System.Linq;

//public class App
//{
//	private readonly LogManager _log;
//    private readonly MainManager _mainManager;
//    private readonly IConfiguration _settings;

//    public App(LogManager log, MainManager mainManager, IConfiguration settings)
//    {
//		_log = log;
//        _mainManager = mainManager;
//        _settings = settings;
//    }
//    internal void Run(string[] args)
//    {

//        //args = new string[3];
//        //args[0] = "--update"; 
//        //args[1] = "-f";
//        //args[2] = @"C:\ZionNet\DevOps\KLA\DataDictionaryValidation-ExampleApp\InputFiles\ATLAS.reassign.xml"; 

//        args = new string[1];
//        args[0] = ""; 

//        //if user acces without args params then start user interface
//        if (args.Length == 0)
//        {
//            Console.ForegroundColor = ConsoleColor.Cyan;
//            Console.WriteLine("Instructions:");
//            Console.WriteLine("Xml Paths Reside in Configuration file:");
//            Console.WriteLine("For Verifying The Content Of Xml Files Use: UniqueIdsScanner.exe --verify");
//            Console.WriteLine("For Updating The Database Use : UniqueIdsScanner.exe --update");
//            Console.WriteLine("Input Xml Path:");
//            Console.ResetColor();

//        }
//        else
//        {
//            using (var parser = new CommandLine.Parser((settings) =>
//            {
//                settings.CaseSensitive = true;
//            }))
//            {
//                //add logs
//                var result = Parser.Default.ParseArguments<CliOptions>(args)
//               .WithParsed(Options => {

//                   // user wanted 1 file option (-f path)
//                   if (!string.IsNullOrWhiteSpace(Options.filePath))
//                   {
//                       RunVerifyAndUpdate(Options.filePath, Options.isUpdate);
//                   }
//                   else //default config mode
//                   {
//                       List<string>? listOfXmlFileFromAppSettings = _settings.GetSection("XmlFilesPath").Get<List<string>>();
//                       if (listOfXmlFileFromAppSettings != null && _mainManager.ValidateXmlFilePaths(listOfXmlFileFromAppSettings))
//                       {
//                           listOfXmlFileFromAppSettings.ForEach(path => RunVerifyAndUpdate(path, Options.isUpdate));
//                       }
//                   }
//               })
//               //if user enterd unkown option/argument
//               .WithNotParsed((errs) => throw new ArgumentException($"Failed to parse command line arguments: {errs}"));
//            }
//        }
//    }


//    public void RunVerifyAndUpdate(string filepath, bool isUpdate)
//    {
//        M_SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);
//        M_SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());

//        Console.WriteLine("verifying: " + filepath.Trim());
//        //verifying
//        if (xmlScopes != null)
//        {
//            // go through all the dictionaries and compare their values with db
//            if (_mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes))
//            {
//                Console.WriteLine("no conflicts");

//                //if user selected the --update cliCommand option
//                if (isUpdate)
//                {
//                    _mainManager.UpdateDatabaseWithNewUniqueIds();
//                }
//            }
//        }
//        else
//        {
//            _log.LogError("Can't seperate xml file to scopes", LogProviderType.Console);
//        }
//    }
//}



