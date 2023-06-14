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

public class App
{
	private readonly LogManager _log;
    private readonly MainManager _mainManager;

    public App(LogManager log, MainManager mainManager)
    {
		_log = log;
        _mainManager = mainManager;
    }
    internal void Run(string[] args)
    {

        args = new string[3];
        args[0] = "--update"; args[1] = "-f"; args[2] = @"E:/CodingPlayground/XMLSerializerExmaple/XmlSerizalizeExample/XmlSerizalizeExample/bin/Debug/net6.0/ATLAS.reassign.xml"; // Uniqe.exe --update -f/-c/-a path = verify&update else = verify
        //args[0] = "-f"; args[1] = "path";

        //if user acces without args params then start user interface
        if (args.Length == 0)
        {
            _log.LogEvent("This is a test event message.", LogProviderType.Console);
            _log.LogEvent("This is a test event message.", LogProviderType.File);


            Console.WriteLine("Hello please enter path");
            var result = Parser.Default.ParseArguments<CliOptions>(args);
        }
        else
        {
            CliOptions cliOptions;
            using (var parser = new CommandLine.Parser((settings) =>
            {
                settings.CaseSensitive = true;
            }))
            {
                //add logs
                var result = Parser.Default.ParseArguments<CliOptions>(args)
               .WithParsed<CliOptions>(Options => {
                   if (string.IsNullOrWhiteSpace(Options.filePath))
                   {
                       Console.WriteLine("Invalid command line arguments");
                       throw new ArgumentException("filePath is null or empty or white-space");                    
                   }
                   else
                   {
                       cliOptions = Options;
                       M_SeperatedScopes? xmlScopes = _mainManager.XmlToSeperatedScopes(cliOptions.filePath);
                       M_SeperatedScopes? DbScopes = _mainManager.SortUniqeIDsFromDbByScope(_mainManager.RetriveUniqeIDsFromDB());
                       
                       Console.WriteLine("verifying: " + cliOptions.filePath.Trim());
                       //verifying
                       if (xmlScopes != null)
                       {
                           // go through all the dictionaries and compare their values with db
                           if (_mainManager.CompareXmlScopesWithDBScopes(xmlScopes, DbScopes))
                           {
                               Console.WriteLine("no conflicts");

                               //if user selected the --update cliCommand option
                               if (cliOptions.isUpdate)
                               {
                                   
                               }
                           } 
                       }
                       else
                       {
                           //cant be null to continue
                           //log
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

