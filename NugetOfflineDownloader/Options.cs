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

        [Value(index: 0, HelpText = "the manifest file containing the output of 'Get-Package | Format-List -Property Id,Version'")]
        public string Manifest { get; set; }
    }
}
