"..\..\_3rd part\NuGet\nuget.exe" pack SPCAFContrib.ReSharper.nuspec -NoPackageAnalysis -Verbosity detailed
"..\..\_3rd part\NuGet\nuget.exe" push SPCAFContrib.ReSharper.1.0.0.32.nupkg -ApiKey 10d0af8b-0530-4889-9241-016c0e64d902 -Source https://resharper-plugins.jetbrains.com
pause