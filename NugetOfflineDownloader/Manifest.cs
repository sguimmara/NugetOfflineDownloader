using NuGet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetOfflineDownloader
{
    public struct PackageInfo
    {
        public PackageInfo(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; }
        public string Version { get; }
    }

    public class Manifest
    {
        private Manifest(IEnumerable<PackageInfo> packages)
        {
            Packages = packages;
        }

        public IEnumerable<PackageInfo> Packages { get; }

        public static Manifest Parse(string path, ILogger logger)
        {
            //Id                   Version
            //--------             -
            //CommandLineParser    2.7.82
            //NuGet.Packaging.Core 5.5.0
            //NuGet.Protocol       5.5.0

            List<PackageInfo> infos = new List<PackageInfo>();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    SkipWhiteLines(reader);
                    SkipHeader(reader);

                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }

                        try
                        {
                            var split = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                            if (split.Length == 2)
                            {
                                infos.Add(new PackageInfo(split[0], split[1]));
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e.Message);
                        }
                    }
                }
            }

            return new Manifest(infos);
        }

        private static void SkipHeader(StreamReader reader)
        {
            reader.ReadLine();
            reader.ReadLine();
        }

        private static void SkipWhiteLines(TextReader reader)
        {
            while (true)
            {
                char c = (char)reader.Peek();
                if (char.IsWhiteSpace(c))
                {
                    reader.Read();
                }
                else
                {
                    return;
                }
            }
        }

    }
}
