using CommandLine.Text;
using CommandLine;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Utility_LOG;
using static Utility_LOG.LogManager;

public class App
{
	private readonly LogManager _log;

	public App(LogManager log)
    {
		_log = log;
	}
    internal void Run(string[] args)
    {
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
				var result = Parser.Default.ParseArguments<CliOptions>(args)
               .WithParsed<CliOptions>(Options => {
                   if (string.IsNullOrWhiteSpace(Options.filePath))
                   {
                       Console.WriteLine("Invalid command line arguments");
                       throw new ArgumentException("filePath is null or empty or white-space");
                   }
                   else { cliOptions = Options; Console.WriteLine(cliOptions.filePath.Trim()); }
               })
               .WithNotParsed((errs) => throw new ArgumentException($"Failed to parse command line arguments: {errs}"));
            }
		}
    }
}

public class CliOptions
{
    [Option('c',"configFilePath", HelpText = @"The base folder path for merging with xml files path in appstetings.json, usage: -c C:\folder\BaseXmlFilesFolder")]
    public string? configFilePath { get; set; }

    [Option('f', "filePath", HelpText = @"The xml file path, usage: -i C:\folder\file.xml")]
    public string? filePath { get; set; }

    [Option ('a',"folderPath",HelpText = @"The path for a folder with xml files, usage: -a C:\folder\OnlyXmlFilesFolder")]
    public string? folderPath { get; set; }
}