﻿using CommandLine;
using Utility_LOG;
using UniqueIdsScannerUI;
using Model;
using Entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text.Json;

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
        _log.LogEvent("App Start Runnig...",LogProviderType.Console);

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
            _log.LogException($"Exception in Run method: {ex.Message}", ex, LogProviderType.File);
            throw;
        }
    }

    private void DisplayInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Welcome to Unique IDs Scanner!");
        Console.WriteLine("================================");
        Console.WriteLine("Instructions:");
        Console.WriteLine("1. Configure XML paths in the appsettings.json configuration file.");
        Console.WriteLine("2. Use the following commands to perform different actions:");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Instructions:");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("--verify: Verify the content of XML files.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --verify");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll --verify -f 'Path To XML File'");
        Console.WriteLine();
        Console.WriteLine("--update: Verify and update the database.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --update");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll --update -f 'Path To XML File'");
        Console.WriteLine();
        Console.WriteLine("--generate-report: Generate a report.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --generate-report");
        Console.WriteLine();
        Console.WriteLine("--update -r: Create a new Alias.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --update -r");
        Console.WriteLine();

        Console.WriteLine("** Please follow the instructions carefully. **");
        Console.WriteLine("==============================================");
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
        try
        {
            if (options.isGenerateReport)
            {
                GenerateReport();
                return;
            }

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

                //throw new ArgumentException("Invalid Xml File Path");
                return;
            }

            foreach (var validXmlFile in validXmlFilePaths)
            {
                ProcessXmlFile(validXmlFile, options);
            }
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in RunOptions method: {ex.Message}", ex, LogProviderType.File);
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

    private void ProcessXmlFile(string filePath, CliOptions options)
    {
        try
        {
            bool getFullInfo = options.Info;
            bool CanBeUpdated = options.isVerify || options.isUpdate ? RunVerify(filePath, getFullInfo) : false;

            if (options.isUpdate)
            {
                RunUpdate(CanBeUpdated, options);
            }
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in ProcessXmlFile method for file {filePath}: {ex.Message}", ex, LogProviderType.File);
            throw;
        }
    }


    private bool RunVerify(string filepath,bool getFullInfo)
    {
        try
        {
            SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);

            if (xmlScopes == null)
            {
                return false;
            }

            SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());
            return _mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes, getFullInfo);
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in RunVerify method for file {filepath}: {ex.Message}", ex, LogProviderType.File);
            throw;
        }
    }

    private void RunUpdate(bool isUpdate, CliOptions options)
    {
        try
        {
            if (isUpdate)
            {
                _mainManager.UpdateDatabaseWithNewUniqueIds();

                if (options.isRenamed)
                {
                    SetUpRename();
                }
            }
        }
        catch (Exception ex)
        {
            _log.LogError($"Error in RunUpdate method: {ex.Message}", LogProviderType.File);
            throw;
        }
       
    }

    private bool isAuthenticatedUser()
    {
        try
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
        catch (Exception)
        {
            throw;
        }
        
    }
    public void GenerateReport()
    {
        try
        {
            string folderForGenerateReports = _settings.GetValue<string>("GenerateReport");

            if (!string.IsNullOrEmpty(folderForGenerateReports))
            {
                int counter = 1;
                string baseFileName = $"{DateTime.Now:dd-MM-yyyy}_Report";
                string extension = ".txt";

                string FileName = baseFileName + extension;
                string tempFilePath = Path.Combine(folderForGenerateReports, FileName);

                while (File.Exists(tempFilePath))
                {
                    FileName = $"{baseFileName}_{counter++}{extension}";
                    tempFilePath = Path.Combine(folderForGenerateReports, FileName);
                }

                _mainManager.GenerateReport(tempFilePath);
            }
            else
            {
                _log.LogError("Error: The folder for report files was not found.", LogProviderType.Console);

            }

        }
        catch (Exception ex)
        {
            _log.LogError($"Error in function GenerateReport(): {ex.Message}",LogProviderType.File);
            throw;
        }
    }

    public void SetUpRename()
    {
        try
        {
            var renameDict = _settings.GetSection("Renamed").Get<Dictionary<string, string>>();
            if (renameDict != null)
            {
                _mainManager.ValidateAndPrepareAliases(renameDict);
            }
            else
            {
                _log.LogWarning(" 'Rename' information in Appsettings.json is empty", LogProviderType.Console);
            }
        }
        catch (Exception ex)
        {
            _log.LogWarning($"Error in SetUpRename method: {ex.Message}", LogProviderType.File);
            throw;
        }

        
    }

}

