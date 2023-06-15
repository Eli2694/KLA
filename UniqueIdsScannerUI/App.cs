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
        //Please follow instructions carefully, Program will now quit.
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
            VerifyAndUpdateXmlFile(options.filePath, options.isUpdate);
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
                VerifyAndUpdateXmlFile(filePath, isUpdate);
            }
        }
    }

    public void VerifyAndUpdateXmlFile(string filepath, bool isUpdate)
    {
        try
        {
            M_SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);
            M_SeperatedScopes? dbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());

            Console.WriteLine($"Now Verifying: {filepath.Trim()}");

            if (xmlScopes != null && _mainManager.CompareXmlScopesWithDBScopes(xmlScopes, dbScopes))
            {
                _log.LogEvent($"There is No conflicts in {filepath.Trim()}",LogProviderType.Console);

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







