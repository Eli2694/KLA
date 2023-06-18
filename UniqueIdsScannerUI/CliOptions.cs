﻿using CommandLine.Text;
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
        [Option('f', "filePath", HelpText = @"The xml file path, usage: -f C:\folder\file.xml")]
        public string? filePath { get; set; }

        [Option(longName: "update", HelpText = @"If you want to verify&update then: UniqueIdsScanner.exe --update")]
        public bool isUpdate { get; set; }

        [Option(longName: "verify", HelpText = @"If you want to verify then: UniqueIdsScanner.exe --verify")]
        public bool isVerify { get; set; }

        [Option(longName: "generate-report", HelpText = @"If you want to generate-report then: UniqueIdsScanner.exe --generate-report")]
        public bool isGenerateReport { get; set; }

        [Option('r', "rename", HelpText = @"If you want to create a new Alias then: UniqueIdsScanner.exe --update -r")]
        public bool isRenamed { get; set; }
    }

}
