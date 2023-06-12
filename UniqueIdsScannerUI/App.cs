using CommandLine.Text;
using CommandLine;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Entity;
using Model;
using System.Linq;
using System.Collections.Generic;
using UniqueIdsScannerUI;

public class App
{
    private readonly MainManager _mainManager;

    public App(MainManager mainManager)
    {
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
                       Dictionary<string, Dictionary<string, M_UniqueIds>>? xmlAsDictionary = _mainManager.XmlToObjectsDictionary(cliOptions.filePath);
                       
                       Console.WriteLine("verifying: " + cliOptions.filePath.Trim());
                       
                       if(xmlAsDictionary != null)
                       {
                           // go through all the dictionaries and compare their values with db
                           foreach (var item in xmlAsDictionary)
                           {
                               string result = item.Value.Aggregate("",
                                           (current, pair) => current + $"{pair.Key}: {pair.Value.Name}, {pair.Value.EntityType}\n");

                               Console.WriteLine(result);
                           }
                       }
                       else
                       {
                           //cant be null to continue
                           //log
                           throw new Exception();
                       }
                       
                           
                        //if user selected the --update cliCommand option
                       if (cliOptions.isUpdate)
                       {
                           Console.WriteLine("updating");
                       }
                   }
               })
                //if user enterd wrong arguments
               .WithNotParsed((errs) => throw new ArgumentException($"Failed to parse command line arguments: {errs}"));
            }
        }
    }
}


