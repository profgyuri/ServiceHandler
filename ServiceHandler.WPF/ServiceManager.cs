using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace ServiceHandler.WPF;

internal class ServiceManager
{
    private const string ServicePrefix = "profgyuri.";
    private const string Create = "create";
    private const string Delete = "delete";

    /// <summary>
    ///     Tries to register a new windows service.
    /// </summary>
    /// <param name="serviceName">Name of the service to register.</param>
    /// <param name="path">Local path of the .exe or .dll file to install the new service from.</param>
    public void AddService(string serviceName, string path)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentNullException(nameof(serviceName));
        }

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (GetServices().Any(x => x == serviceName))
        {
            return;
        }

        var info = GetProcessInfo(Create, $"{ServicePrefix}{serviceName}");
        info.Arguments += path;

        var process = Process.Start(info);
        process?.WaitForExit();
    }

    /// <summary>
    ///     Stops a running windows service.
    /// </summary>
    /// <param name="serviceName">Name of the service to stop.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="serviceName" /> is not valid.</exception>
    public void RemoveService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentNullException(nameof(serviceName));
        }

        if (GetServices().All(x => x != serviceName))
        {
            return;
        }

        var info = GetProcessInfo(Delete, $"{ServicePrefix}{serviceName}");

        var process = Process.Start(info);
        process?.WaitForExit();
    }

    public IEnumerable<string> GetServices()
    {
        var services =
            ServiceController
                .GetServices()
                .Where(x => x.ServiceName.StartsWith(ServicePrefix))
                .Select(x => x.ServiceName[(ServicePrefix.Length - 1)..]);

        return services;
    }

    private ProcessStartInfo GetProcessInfo(string type, string serviceName)
    {
        return new ProcessStartInfo
        {
            UseShellExecute = true,
            RedirectStandardOutput = false,
            FileName = "powershell.exe",
            Arguments = "sc.exe " + type + $" \"{serviceName}\"",
            CreateNoWindow = true
        };
    }
}