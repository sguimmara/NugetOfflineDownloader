using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;

namespace NugetOfflineDownloader
{
    public sealed class ConsoleLogger : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            Console.WriteLine($"{message.Level.ToString().ToUpperInvariant(),-12} {message.Message}");
        }

        public override Task LogAsync(ILogMessage message)
        {
            return Task.Run(() =>
            {
                Log(message);
            });
        }
    }
}
