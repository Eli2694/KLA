﻿using CommandLine;
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
        args = new string[1];
        args[0] = "--generate-report";
        try
        {
            if (args.Length == 0)
            {
                DisplayInstructions();
            }
            else
            {
                if (isAuthenticatedUser())
                {
                    ParseArgumentsAndRunOptions(args);
                }
            }
        }
        catch (Exception ex)
        {
            _log.LogException("Exception in Run method", ex, LogProviderType.File);
        }
    }

    private void DisplayInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Welcome to Unique IDs Scanner!");
        Console.WriteLine("================================");
        Console.WriteLine("Instructions:");
        Console.WriteLine("1. Configure XML paths in the appsettings.json configuration file.");
        Console.WriteLine("2. If you want to only verify the content of XML files, use:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --verify");
        Console.WriteLine("   or specify a specific file with:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --verify -f 'Path To XML File'");
        Console.WriteLine("3. If you want to verify and update the database, use:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --update");
        Console.WriteLine("   or specify a specific file with:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --update -f 'Path To XML File'");
        Console.WriteLine();
        Console.WriteLine("Example Usages:");
        Console.WriteLine("---------------");
        Console.WriteLine("1. Verifying XML content using paths from the appsettings.json:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --verify");
        Console.WriteLine();
        Console.WriteLine("2. Verifying specific XML file:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --verify -f 'C:\\folder\\file.xml'");
        Console.WriteLine();
        Console.WriteLine("3. Updating the database using paths from the appsettings.json:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --update");
        Console.WriteLine();
        Console.WriteLine("4. Updating the database with a specific XML file:");
        Console.WriteLine("   UniqueIdsScannerUI.exe --update -f 'C:\\folder\\file.xml'");
        Console.WriteLine();
        Console.WriteLine("** Please follow the instructions carefully. **");
        Console.WriteLine("==============================================");
        Console.WriteLine("Press any key to quit.");
        Console.ResetColor();
        Console.ReadKey();
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
                string errorMessage = $"Invalid File Path: {xmlFile}";
                _log.LogError(errorMessage, LogProviderType.Console);
                _log.LogError(errorMessage, LogProviderType.File);
                inValidXmlFilePaths.Add(xmlFile);
            }
        }

        if (inValidXmlFilePaths.Any())
        {

            throw new ArgumentException();
        }

        foreach (var validXmlFile in validXmlFilePaths)
        {
            ProcessXmlFile(validXmlFile, options);
        }
    }


    private void ProcessXmlFile(string filePath, CliOptions options)
    {
        try
        {
            bool CanBeUpdated = options.isVerify || options.isUpdate ? RunVerify(filePath) : false;

            if (options.isUpdate)
            {
                RunUpdate(CanBeUpdated);
            }
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in ProcessXmlFile method for file {filePath}", ex, LogProviderType.File);
            throw;
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
        try
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
        catch (Exception ex)
        {
            _log.LogException($"Exception in RunVerify method for file {filepath}", ex, LogProviderType.File);
            throw;
        }
    }

    private void RunUpdate(bool isUpdate)
    {
        if (isUpdate)
        {
            _mainManager.UpdateDatabaseWithNewUniqueIds();
        }
    }

    private bool isAuthenticatedUser()
    {
        List<string>? NameAndPass = _settings.GetSection("UsernameAndPassword").Get<List<string>>();
        if (NameAndPass != null)
        {
            if (_mainManager.isAuthenticatedUser(NameAndPass))
            {
                return true;
            }
            _log.LogError("Invalid Username Or Password", LogProviderType.Console);
        }
        return false;
    }
    public void GenerateReport()
    {

    }
}

