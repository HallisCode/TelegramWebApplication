using System;
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
        public ILoggingBuilder Logging { get; protected set; } = new LoggingBuilder();
        public IServiceCollection Services { get; protected set; } = new ServiceCollection();

        public IServer? Server { get; protected set; }


        public TelegramWebApplicationBuilder()
        {
            Logging.AddConsole();
        }

        public void SetServer(IServer server)
        {
            Server = server;
        }

        public TelegramWebApplication Build()
        {
            // Добавляем логгирование
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                configure => configure.Services.Add(Logging.Services)
            );
            Services.AddSingleton<ILoggerFactory>(loggerFactory);
            Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

            // Проверяем наличие сервера
            if (Server is null)
            {
                throw new Exception($"Не настроен {nameof(Server)}, необходимо вызвать {nameof(SetServer)}.");
            }

            // Добавляем ServiceProvider
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            Services.AddSingleton<IServiceProvider>(serviceProvider);

            // Создаём приложение
            TelegramWebApplication application = new TelegramWebApplication(Server, Services.BuildServiceProvider());

            Services.Clear();
            Server = null;

            return application;
        }
    }
}