using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting
{
    public static class UnobservedExceptionsHandlerExtension
    {
        public static void InitializeUnobserveExceptionHandler(this IServiceProvider provider)
        {
            _ = provider.GetService<UnobservedExceptionsHandler>();
        }

        public static IHostBuilder UseUnobserveExceptionHandler(this IHostBuilder builder)
            => builder.ConfigureServices((context, services) => services.AddSingleton<UnobservedExceptionsHandler>());
    }

    public class UnobservedExceptionsHandler : IDisposable
    {
        private readonly ILogger _logger;

        public UnobservedExceptionsHandler(ILogger<UnobservedExceptionsHandler> logger)
        {
            _logger = logger;
            AppDomain.CurrentDomain.UnhandledException += DomainUnobservedExceptionHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
        }

        private void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var aggrException = e.Exception;
            var baseException = aggrException.GetBaseException();
            var tplTask = (Task)sender;
            _logger.LogError($"Capture {nameof(UnobservedTaskExceptionEventArgs)} , Aggr:{aggrException} , Base:{baseException} , Task {tplTask}");
        }

        private void DomainUnobservedExceptionHandler(object context, UnhandledExceptionEventArgs args)
        {
            var exception = (Exception)args.ExceptionObject;
            _logger.LogError($"Capture {nameof(DomainUnobservedExceptionHandler)}", exception);
        }

        public void Dispose()
        {
            TaskScheduler.UnobservedTaskException -= UnobservedTaskExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException -= DomainUnobservedExceptionHandler;
        }
    }
}
