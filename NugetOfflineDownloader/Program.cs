using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using CommandLine;

namespace NugetOfflineDownloader
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    Task.Run(() => Run(o)).Wait();
                })
                .WithNotParsed<Options>(e =>
                {
                    Console.Error.WriteLine(e.ToString());
                    Environment.Exit(1);
                });

            return 0;
        }

        public static async Task Run(Options options)
        {
            ILogger logger = new ConsoleLogger();
            Application nuget = new Application(options, logger);

            await nuget.Initialize(new CancellationTokenSource(4000).Token);

            foreach (var pkg in Manifest.Parse(options.Manifest, logger).Packages)
            {
                await nuget.DownloadPackageAsync(pkg.Id, pkg.Version, new CancellationTokenSource(10 * 60 * 1000).Token);
            }
        }
    }
}
