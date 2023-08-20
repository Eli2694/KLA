using CommandLine;
using Utility_LOG;
using UniqueIdsScannerUI;
using Model;
using Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Polly;

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
        _log.LogInfo("App Start Running...",LogProviderType.Console);

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
					_log.LogEvent("User successfuly authenticated.", LogProviderType.Console);
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
        Console.WriteLine("2. Use the following commands to perform different actions:\n");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Command Options:");
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine("--verify: Verify the content of XML files.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --verify");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll --verify -f 'Path To XML File'");
        Console.WriteLine("   Explanation: Verifies XML file content against the database.\n");

        Console.WriteLine("--update: Verify and update the database.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --update");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll --update -i");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll --update -f 'Path To XML File'");
        Console.WriteLine("   Explanation: Verifies XML files and updates the database.");
        Console.WriteLine("                Use the '-i' option to display detailed information");
        Console.WriteLine("                about the verification and update process in the console.\n");

        Console.WriteLine("--generate-report: Generate a report.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --generate-report");
        Console.WriteLine("   Explanation: Generates a report based on verified and updated data in the database.\n");

        Console.WriteLine("--rename || -r: Create a new Alias.");
        Console.WriteLine("   Usage: dotnet UniqueIdsScannerUI.dll --rename");
        Console.WriteLine("          dotnet UniqueIdsScannerUI.dll -r");
        Console.WriteLine("   Explanation: Adds a new Alias.\n");

        Console.WriteLine("** Note: Please follow the instructions carefully. **");
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
            // Generate report if requested
            if (options.isGenerateReport)
            {
                GenerateReport();
                return;
            }

            // Get a list of XML file paths
            List<string> xmlFilePaths = GetFilePaths(options);
            if(xmlFilePaths.Count == 0) 
            {
                _log.LogError($"File paths are not found", LogProviderType.Console);
                return;
            }



            // Validate XML file paths 
            List<string> validXmlFilePaths = new List<string>();
            List<string> invalidXmlFilePaths = new List<string>();
            ValidateXmlFilePaths(xmlFilePaths, validXmlFilePaths, invalidXmlFilePaths);


            if (invalidXmlFilePaths.Any())
            {
                return;
            }

            // Process each valid XML file
            ProcessValidXmlFiles(validXmlFilePaths, options);

            // Add aliases from config file if requested
            if (options.isRenamed)
            {
                SetUpRename();
            }

        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in RunOptions method: {ex.Message}", ex, LogProviderType.File);
            return;
        }   
    }

    private void ProcessValidXmlFiles(List<string> validFilePaths, CliOptions options)
    {
        try
        {
            foreach (var validXmlFile in validFilePaths)
            {
                ProcessXmlFile(validXmlFile, options);
            }
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception while processing XML file: {ex.Message}", ex, LogProviderType.File);
            throw;
        }   
    }

    private void ValidateXmlFilePaths(List<string> filePaths, List<string> validPaths, List<string> invalidPaths)
    {
        try
        {
            foreach (var filePath in filePaths)
            {
                if (_mainManager.ValidateXmlFilePath(filePath))
                {
                    validPaths.Add(filePath);
                }
                else
                {
                    invalidPaths.Add(filePath);
                }
            }
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception while validating XML file path: {ex.Message}", ex, LogProviderType.File);
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

            var retryPolicy = Policy.Handle<DbUpdateConcurrencyException>()
                .Or<DbUpdateException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
                {
                    _log.LogWarning($"Retry attempt {retryCount}", LogProviderType.Console);
                    _log.LogWarning($"Retrying in {timeSpan.TotalSeconds} seconds...", LogProviderType.Console);
                    _log.LogWarning($"Retry attempt {retryCount} due to concurrency error: {exception.Message}", LogProviderType.File);

                });

            retryPolicy.Execute(() =>
            {
                VerifyAndUpdate(filePath, getFullInfo, options.isUpdate, options);
            });

        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in ProcessXmlFile method for file {filePath}: {ex.Message}", ex, LogProviderType.File);
            throw;
        }
    }

    private void VerifyAndUpdate(string filePath, bool getFullInfo, bool isUpdate, CliOptions options)
    {
        try
        {
            bool CanBeUpdated = options.isVerify || options.isUpdate ? RunVerify(filePath, getFullInfo) : false;


            if (options.isUpdate)
            {
                RunUpdate(CanBeUpdated, options, filePath);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            _log.LogWarning($"Start retry policy because of DbUpdateConcurrencyException", LogProviderType.Console);
            throw;
        }
        catch (DbUpdateException)
        {
            _log.LogWarning($"Start retry policy because of DbUpdateException", LogProviderType.Console);
            throw;
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in VerifyAndUpdate method for file {filePath}: {ex.Message}", ex, LogProviderType.File);
            throw;
            
        }   
    }

    private bool RunVerify(string filepath,bool getFullInfo)
    {
        try
        {
            // Seperate XML data to lists by scope
            SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(filepath);

            if (xmlScopes == null)
            {
                return false;
            }

            // Check if the names in XML file are not found in aliases table under the same scope
            bool notFound = _mainManager.CheckIfNamesExistInAliasLists(xmlScopes);

            if (notFound)
            {
                // Seperate Unique Ids from database to lists by scope
                SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());

                // return true if there are no errors between data from Xml File and data from Database
                return _mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes, getFullInfo, filepath);
            }
            else
            {
                return false;
            }
            
        }
        catch (Exception ex)
        {
            _log.LogException($"Exception in RunVerify method for file {filepath}: {ex.Message}", ex, LogProviderType.File);
            throw;
        }
    }

    private void RunUpdate(bool isUpdate, CliOptions options, string filePath)
    {
        try
        {
            if (isUpdate)
            {   
                 string fileName = Path.GetFileName(filePath);
                _log.LogEvent($"Updating File: {fileName}", LogProviderType.Console);

                // Update database with unique ids 
                _mainManager.UpdateDatabaseWithNewUniqueIds();
            }
        }
        catch (DbUpdateConcurrencyException)
        {

            throw;
        }
        catch (DbUpdateException)
        {

            throw;
        }
        catch (Exception ex)
        {
            _log.LogError($"Error in RunUpdate method: {ex.Message}", LogProviderType.File);
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
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.LogWarning($"Error in SetUpRename method: {ex.Message}", LogProviderType.File);
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

            // Get the array from the configuration
            string[] usernameAndPasswordArray = _settings.GetSection("UsernameAndPassword").Get<string[]>();
            // Get the username from the array (assuming it's the first element in the array)
            string username = usernameAndPasswordArray[0];

            if (!string.IsNullOrEmpty(folderForGenerateReports))
            {
                int counter = 1;
                string baseFileName = $"{username}_Report_{DateTime.Now:dd-MM-yyyy}";
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

}

