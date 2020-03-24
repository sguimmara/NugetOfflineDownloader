using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace NugetOfflineDownloader
{
    public sealed class Options
    {
        [Option(shortName: 'o', Default = "packages", Required = false, HelpText = "the directory to put the downloaded packages")]
        public string OutputDirectory { get; set; }

        [Value(index: 0, HelpText = "the manifest file containing the output of 'Get-Package | Select-Object Id,Version -Unique'")]
        public string Manifest { get; set; }

        [Option(shortName:'f', Default = ".NETFramework", HelpText = "list of accepted frameworks separated by commas")]
        public string Frameworks { get; set; }
    }
}
