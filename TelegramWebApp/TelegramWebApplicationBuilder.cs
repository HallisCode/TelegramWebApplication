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
            _SetUpDefaultSettings();
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

            // Внедряем логгирование
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                configure => configure.Services.Add(Logging.Services)
            );
            Services.AddSingleton<ILoggerFactory>(loggerFactory);
            Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

            // Внедряем конфигуратор
            IConfigurationRoot configurationRoot = Configuration.Build();
            Services.AddSingleton<IConfiguration>(configurationRoot);

            // Внедряем провайдера в самого себя
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            Services.AddSingleton<IServiceProvider>(serviceProvider);

            // Создаём приложение
            TelegramWebApplication application = new TelegramWebApplication(
                server: Server,
                provider: Services.BuildServiceProvider(),
                logger: loggerFactory.CreateLogger<TelegramWebApplication>(),
                configuration: configurationRoot);

            // Сбрасываем все накопленные настройки
            Logging.ClearProviders();
            Configuration = new ConfigurationManager();
            Services.Clear();
            Server = null;

            return application;
        }

        protected virtual void _SetUpDefaultSettings()
        {
            _LoggingDefaultConfigure();
            _ConfigurationDefaultConfigure();
        }

        protected virtual void _ConfigurationDefaultConfigure()
        {
            Configuration.AddEnvironmentVariables();
        }

        protected virtual void _LoggingDefaultConfigure()
        {
            Logging.AddConsole();
        }
    }
}