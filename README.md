# NugetOfflineDownloader

This application is designed to download missing packages from an offline Visual Studio solution.

## How to use

1. Open the solution with Visual Studio
1. Open the nuget console (`Tools` > `NuGet Package manager` > `Console`)
1. Type

    ```powershell
    Get-Package | Select-Object Id,Version -Unique > packages.txt
    ```

1. Copy the `packages.txt` file to the online computer
1. Execute

    ```shell
    NugetOfflineDownloader.exe packages.txt -o packages [-f .NETFramework] [-u https://customNugetRepository]
    ```

    To download all packages and their dependency into the `packages` directory, filtering only the framework `.NETFramework` (excluding .NET core versions for example).

    You can specify with -u option a custom Nuget repository, default is `https://api.nuget.org/v3/index.json`.

1. Copy the `packages` directory to the offline computer
1. Add this directory as a nuget source (see <https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio> for more information)
