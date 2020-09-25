﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NugetOfflineDownloader
{
    public class Application
    {
        private readonly string _localCache;
        private readonly SourceCacheContext _cache;
        private readonly SourceRepository _repository;
        private readonly ILogger _logger;
        private readonly HashSet<string> _acceptedFrameworks = new HashSet<string> { "Any" };

        private FindPackageByIdResource _resource;

        public Application(Options options, ILogger logger)
        {
            CancellationToken cancellationToken = CancellationToken.None;

            _acceptedFrameworks.AddRange(options.Frameworks.Split(',').Select(s => s.Trim()));

            _localCache = options.OutputDirectory;
            _cache = new SourceCacheContext();
            _repository = Repository.Factory.GetCoreV3(options.NugetServiceUrl);
            _logger = logger;

            Directory.CreateDirectory(options.OutputDirectory);
        }

        public async Task Initialize(CancellationToken ct)
        {
            _resource = await _repository.GetResourceAsync<FindPackageByIdResource>(ct);
        }

        public async Task DownloadPackageAsync(PackageIdentity packageId, CancellationToken ct)
        {
            string outputFile = Path.Combine(_localCache, $"{packageId}.nupkg");
            if (!File.Exists(outputFile))
            {
                try
                {
                    bool exists = await _resource.DoesPackageExistAsync(packageId.Id, packageId.Version, _cache, _logger, ct);
                    if (exists)
                    {
                        var dl = await _resource.GetPackageDownloaderAsync(packageId, _cache, _logger, ct);
                        await dl.CopyNupkgFileToAsync(outputFile, ct);
                    }
                    else
                    {
                        _logger.LogError($"the package {packageId} does not exist on {_repository.PackageSource}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
            else
            {
                _logger.LogInformation($"  SKIP {packageId} (already in cache)");
            }

            await DownloadDependencies(packageId, ct);
        }

        private async Task DownloadDependencies(PackageIdentity packageId, CancellationToken ct)
        {
            var deps = await _resource.GetDependencyInfoAsync(packageId.Id, packageId.Version, _cache, _logger, ct);

            if (deps != null && deps.DependencyGroups != null)
            {
                foreach (var dg in deps.DependencyGroups)
                {
                    if (!_acceptedFrameworks.Contains(dg.TargetFramework.Framework))
                    {
                        continue;
                    }
                    foreach (var dep in dg.Packages)
                    {
                        var depVersion = dep.VersionRange.MinVersion;

                        var depPackageId = new PackageIdentity(dep.Id, depVersion);

                        try
                        {
                            await DownloadPackageAsync(depPackageId, ct);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                        }
                    }
                }
            }
        }

        public Task DownloadPackageAsync(string id, string version, CancellationToken ct)
        {
            var packageId = new PackageIdentity(id, new NuGet.Versioning.NuGetVersion(version));
            return DownloadPackageAsync(packageId, ct);
        }

        public async Task DownloadPackagesAsync(IEnumerable<Tuple<string, string>> packages, CancellationToken ct)
        {
            foreach (var tuple in packages)
            {
                string id = tuple.Item1;
                string version = tuple.Item2;

                try
                {
                    await DownloadPackageAsync(id, version, ct);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }
    }
}
