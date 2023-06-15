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
        Console.WriteLine("Instructions:");
        Console.WriteLine("Xml Paths Reside in Configuration file:");
        Console.WriteLine("For Verifying The Content Of Xml Files Use: UniqueIdsScanner.exe --verify");
        Console.WriteLine("For Updating The Database Use : UniqueIdsScanner.exe --update");
        Console.WriteLine("Input Xml Path:");
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
        // Provide meaningful error message to user and/or log it.
        // ...
    }

    private void RunOptions(CliOptions options)
    {
        List<string> xmlFilePaths = GetFilePaths(options);
        List<string> validXmlFilePaths = new List<string>();

        foreach (var xmlFile in xmlFilePaths)
        {
            if (_mainManager.ValidateXmlFilePath(xmlFile))
            {
                validXmlFilePaths.Add(xmlFile);
            }
            else
            {
                _log.LogError($"Invalid File Path: {xmlFile}", LogProviderType.Console);
            }
        }

        if (validXmlFilePaths.Count == 0)
        {
            throw new ArgumentException("No valid file paths found");
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
        M_SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);

        if (xmlScopes == null)
        {
            _log.LogError("Can't separate xml file to scopes", LogProviderType.Console);
            return false;
        }

        M_SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());
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


