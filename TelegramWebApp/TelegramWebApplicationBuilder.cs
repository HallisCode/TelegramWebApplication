using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SimpleNetFramework.Core;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure.Logging;

namespace TelegramWebApp
{
    public class TelegramWebApplicationBuilder : IWebApplicationBuilder<TelegramWebApplication>
    {
        public IConfigurationManager Configuration { get; protected set; } = new ConfigurationManager();
        public ILoggingBuilder Logging { get; protected set; } = new LoggingBuilder();
        public IServiceCollection Services { get; protected set; } = new ServiceCollection();

        public IServer? Server { get; protected set; }


        public TelegramWebApplicationBuilder()
        {
            
        }

        public void SetServer(IServer server)
        {
            Server = server;
        }

        public TelegramWebApplication Build()
        {
            // Проверяем наличие сервера
            if (Server is null)
            {
                throw new Exception($"Не настроен {nameof(Server)}, необходимо вызвать {nameof(SetServer)}.");
            }

            ILoggerFactory loggerFactory = _LoggingConfigure();
            ILogger loggerDefault = loggerFactory.CreateLogger<TelegramWebApplication>();

            IConfigurationRoot configurationRoot = _ConfigurationConfigure();

            IServiceProvider serviceProvider = _ConfigureProvider();

            // Создаём приложение
            TelegramWebApplication application = new TelegramWebApplication(
                server: Server,
                provider: serviceProvider,
                logger: loggerDefault,
                configuration: configurationRoot);

            // Сбрасываем все накопленные настройки
            Logging.ClearProviders();
            Configuration = new ConfigurationManager();
            Services.Clear();
            Server = null;

            return application;
        }

        protected virtual ILoggerFactory _LoggingConfigure()
        {
            // Добавляем провайдеры по умолчанию
            Logging.AddConsole();

            // Внедряем логгирование
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                configure => configure.Services.Add(Logging.Services)
            );
            Services.AddSingleton<ILoggerFactory>(loggerFactory);
            Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

            return loggerFactory;
        }

        protected virtual IConfigurationRoot _ConfigurationConfigure()
        {
            // Добавляем провайдеры по умолчанию
            Configuration.AddEnvironmentVariables();

            // Внедряем конфигурацию
            IConfigurationRoot configurationRoot = Configuration.Build();

            Services.AddSingleton<IConfiguration>(configurationRoot);

            return configurationRoot;
        }

        /// <summary>
        /// Должен вызываться самым последним в <see cref="Build"/>.
        /// </summary>
        protected virtual IServiceProvider _ConfigureProvider()
        {
            // Добавляем ServiceProvider
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            Services.AddSingleton<IServiceProvider>(serviceProvider);

            return Services.BuildServiceProvider();
        }
    }
}