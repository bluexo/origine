using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;

namespace Origine.Control.Services
{
    public class ServerControlService
    {
        public IEnumerable<Process> Servers => _servers.Where(s => !s.HasExited);

        readonly List<Process> _servers = new List<Process>();
        readonly IConfiguration _configuration;
        readonly IWebHostEnvironment _hostEnvironment;

        public ServerControlService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _hostEnvironment = env;
        }

        public void StartProcess()
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = Path.Combine(_hostEnvironment.WebRootPath, _configuration["ServerControl:WorkDir"] ?? "server"),
                Arguments = _configuration["ServerControl:FileName"] ?? "server.exe",
                FileName = "dotnet",
            };

            var process = Process.Start(startInfo);
            process.Exited += OnExited;
            _servers.Add(process);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine($"Process {process.ProcessName}:{process.Id} started ");
        }

        private void OnExited(object sender, EventArgs e)
        {
            var process = sender as Process;

            _servers.Remove(process);
        }

        private void OnProcessOutput(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"{e.Data}");
        }
    }
}
