using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace ServiceHandler.WPF;

internal class ServiceManager
{
    private const string ServicePrefix = "profgyuri-";
    private const string Create = "create";
    private const string Delete = "delete";
    private const string Start = "start";
    private const string Stop = "stop";

    /// <summary>
    ///     Tries to register a new windows service.
    /// </summary>
    /// <param name="serviceName">Name of the service to register.</param>
    /// <param name="path">Local path of the .exe or .dll file to install the new service from.</param>
    public void AddService(string serviceName, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found on given path!", path);
        }

        switch (Path.GetExtension(path))
        {
            case ".exe":
            case ".dll":
                break;
            default:
                throw new InvalidOperationException("Invalid file chosen! Only .exe and .dll files are supported!");
        }

        if (GetServices().Any(x => x == serviceName))
        {
            return;
        }

        BasicServiceCommand(serviceName, Create, path);
    }

    /// <summary>
    ///     Stops a running windows service.
    /// </summary>
    /// <param name="serviceName">Name of the service to stop.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="serviceName" /> is not valid.</exception>
    public void RemoveService(string serviceName)
    {
        BasicServiceCommand(serviceName, Delete);
    }

    /// <summary>
    ///     Starts a windows service.
    /// </summary>
    /// <param name="serviceName">Name of the windows service to start.</param>
    public void StartService(string serviceName)
    {
        BasicServiceCommand(serviceName, Start);

        var serviceController = new ServiceController(ServicePrefix + serviceName);
        serviceController.WaitForStatus(ServiceControllerStatus.Running);
    }

    /// <summary>
    ///     Stops a windows service.
    /// </summary>
    /// <param name="serviceName">Name of the windows service to stop.</param>
    public void StopService(string serviceName)
    {
        BasicServiceCommand(serviceName, Stop);

        var serviceController = new ServiceController(ServicePrefix + serviceName);
        serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    private void BasicServiceCommand(string serviceName, string command, string path = "")
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentNullException("The name of the service cannot be empty!",
                new ArgumentNullException(nameof(serviceName)));
        }

        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentNullException(nameof(command));
        }

        var info = GetProcessInfo(command, $"{ServicePrefix}{serviceName}", path);

        var process = Process.Start(info);
        process?.WaitForExit();
    }

    public IEnumerable<string> GetServices()
    {
        var services =
            ServiceController
                .GetServices()
                .Where(x => x.ServiceName.StartsWith(ServicePrefix))
                .Select(x => x.ServiceName[ServicePrefix.Length..]);

        return services;
    }

    private ProcessStartInfo GetProcessInfo(string type, string serviceName, string path = "")
    {
        return new ProcessStartInfo
        {
            UseShellExecute = true,
            RedirectStandardOutput = false,
            FileName = "powershell.exe",
            Arguments = string.IsNullOrWhiteSpace(path)
                ? $"sc.exe {type} \"{serviceName}\""
                : $"sc.exe {type} \"{serviceName}\" binpath=\"{path}\"",
            CreateNoWindow = true,
            Verb = "runas"
        };
    }
}