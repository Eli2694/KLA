using CommandLine;

namespace UniqueIdsScannerUI
{
    public class CliOptions
    {
        [Option('f', "filePath", HelpText = @"The xml file path, usage: -f C:\folder\file.xml")]
        public string? filePath { get; set; }

        [Option(longName: "update", HelpText = @"If you want to verify&update then: dotnet UniqueIdsScanner.dll --update")]
        public bool isUpdate { get; set; }

        [Option(longName: "verify", HelpText = @"If you want to verify then: dotnet UniqueIdsScanner.dll --verify")]
        public bool isVerify { get; set; }

        [Option(longName: "generate-report", HelpText = @"If you want to generate-report then: dotnet UniqueIdsScanner.dll --generate-report")]
        public bool isGenerateReport { get; set; }

        [Option('r', "rename", HelpText = @"If you want to create a new Alias then: dotnet UniqueIdsScanner.dll --update -r")]
        public bool isRenamed { get; set; }

        [Option('i', "info", HelpText = @"If you want to more information about new ids in console: dotnet UniqueIdsScanner.dll --verify -i")]
        public bool Info { get; set; }
    }

}
