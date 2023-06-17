using CommandLine;
using Utility_LOG;
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

            DisplayInstructions();
        }
        else
        {
            ParseArgumentsAndRunOptions(args);
        }
    }

    private void DisplayInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Welcome to Unique IDs Scanner!");
        Console.WriteLine("================================");
        Console.WriteLine("Instructions:");
        Console.WriteLine("1. Configure XML paths in the configuration file.");
        Console.WriteLine("2. To verify the content of XML files, use: UniqueIdsScanner.exe --verify");
        Console.WriteLine("3. To update the database, use: UniqueIdsScanner.exe --update");
        Console.WriteLine();
        Console.WriteLine("Example Usages:");
        Console.WriteLine("---------------");
        Console.WriteLine("1. Verifying XML content:");
        Console.WriteLine("   UniqueIdsScanner.exe --verify -f 'Path To XML File'");
        Console.WriteLine();
        Console.WriteLine("2. Updating the database:");
        Console.WriteLine("   UniqueIdsScanner.exe --update -f 'Path To XML File'");
        Console.WriteLine();
        Console.WriteLine("** Please follow the instructions carefully. **");
        Console.WriteLine("==============================================");
        Console.WriteLine("Press any key to quit.");
        Console.ResetColor();
    }

    private void ParseArgumentsAndRunOptions(string[] args)
    {
        using (var parser = new CommandLine.Parser((settings) => { settings.CaseSensitive = true; }))
        {
            Parser.Default.ParseArguments<CliOptions>(args)
                .WithParsed<CliOptions>(options => RunOptions(options))
                .WithNotParsed(HandleParseError);
        }
    }

    private void HandleParseError(IEnumerable<Error> errors)
    {
        throw new ArgumentException($"Failed to parse command line arguments: {string.Join(", ", errors)}");
    }

    private void RunOptions(CliOptions options)
    {
        List<string> xmlFilePaths = GetFilePaths(options);
        List<string> validXmlFilePaths = new List<string>();
        List<string> inValidXmlFilePaths = new List<string>();

        foreach (var xmlFile in xmlFilePaths)
        {
            if (_mainManager.ValidateXmlFilePath(xmlFile))
            {
                validXmlFilePaths.Add(xmlFile);
            }
            else
            {
                _log.LogError($"Invalid File Path: {xmlFile}", LogProviderType.Console);
                _log.LogError($"Invalid File Path: {xmlFile}", LogProviderType.File);
                inValidXmlFilePaths.Add(xmlFile);
            }
        }

        if (inValidXmlFilePaths.Any())
        {
            throw new ArgumentException("invalid file path was found");
        }

        foreach (var validXmlFile in validXmlFilePaths)
        {
            ProcessXmlFile(validXmlFile, options);
        }
    }

    private void ProcessXmlFile(string filePath, CliOptions options)
    {
        bool CanBeUpdated = options.isVerify || options.isUpdate ? RunVerify(filePath) : false;

        if (options.isUpdate)
        {
            RunUpdate(CanBeUpdated);
        }
    }

    private List<string> GetFilePaths(CliOptions options)
    {
        if (options.filePath != null)
        {
            return new List<string> { options.filePath };
        }
        else
        {
            return _settings.GetSection("XmlFilesPath").Get<List<string>>();
        }
    }

    private bool RunVerify(string filepath)
    {
        SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);

        if (xmlScopes == null)
        {
            _log.LogError($"Can't separate xml file to scopes: {filepath}", LogProviderType.Console);
            _log.LogError($"Can't separate xml file to scopes: {filepath}", LogProviderType.File);
            return false;
        }

        SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());
        return _mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes);
    }

    private void RunUpdate(bool isUpdate)
    {
        if (isUpdate)
        {
            _mainManager.UpdateDatabaseWithNewUniqueIds();
        }
    }
}

