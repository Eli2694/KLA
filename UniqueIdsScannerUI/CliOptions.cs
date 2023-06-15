using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueIdsScannerUI
{
    public class CliOptions
    {
    //  [Option('c', "configFilePath", HelpText = @"The base folder path for merging with xml files path in appstetings.json, usage: -c C:\folder\BaseXmlFilesFolder")]
    //  public string? configFilePath { get; set; }

        [Option('f', "filePath", HelpText = @"The xml file path, usage: -i C:\folder\file.xml")]
        public string? filePath { get; set; }

     // [Option('a', "folderPath", HelpText = @"The path for a folder with xml files, usage: -a C:\folder\OnlyXmlFilesFolder")]
     // public string? folderPath { get; set; }

        [Option(longName: "update", HelpText = @"If you want to verify&update then: -u -(c/f/a) c:\path  OR  --update -(c/f/a) c:\path")]
        public bool isUpdate { get; set; }

        [Option(longName: "verify", HelpText = @"If you want to verify then: UniqueIdsScanner.exe --verify")]
        public bool isVerify { get; set; }
    }
}
