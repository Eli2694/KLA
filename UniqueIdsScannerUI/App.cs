﻿using CommandLine.Text;
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







