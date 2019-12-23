using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Net.Http;

namespace Origine
{
    public static class ApplicationBuilderExtensions
    {
        private static IConfiguration configuration;
        private static IFileProvider fileProvider;
        private static IDisposable changeToken;
        private static DateTime prevChangeTime;
        private readonly static HttpClient httpClient = new HttpClient();

        public static void UsePackageWatcher(this IApplicationBuilder builder, IConfiguration config)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appdata", "game");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            fileProvider = new PhysicalFileProvider(path);
            configuration = config;
            RegisterWatchCallback();
        }

        private static void RegisterWatchCallback()
        {
            var packageFile = configuration.GetValue<string>("WatchedPackageFile");
            var fileInfo = fileProvider.GetFileInfo(packageFile);
            var token = fileProvider.Watch(packageFile);
            changeToken = token.RegisterChangeCallback(OnPackageChange, fileInfo);
        }

        private static async void OnPackageChange(object obj)
        {
            var fileInfo = obj as IFileInfo;
            if (fileInfo.Exists)
            {
                var reqUrl = configuration.GetValue<string>("PackageSignUrl");
                try { await httpClient.GetAsync(reqUrl); }
                catch { ConsoleLogger.WriteError($"Cannot request on {nameof(OnPackageChange)} ,url: {reqUrl}"); }
                ConsoleLogger.WriteStatus($"Package update,$Name={fileInfo.Name}, $length:{fileInfo.Length / 1000_000}M, $LastModified:{fileInfo.LastModified}");
            }
            changeToken?.Dispose();
            RegisterWatchCallback();
        }
    }
}
