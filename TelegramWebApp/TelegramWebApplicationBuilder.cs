using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleNetFramework.Core;
using SimpleNetFramework.Core.Server;

namespace TelegramWebApp
{
    public class TelegramWebApplicationBuilder : IWebApplicationBuilder<TelegramWebApplication>
    {
        private IWebApplicationBuilder<TelegramWebApplication> _webApplicationBuilderImplementation;
        public IServiceCollection Services { get; protected set; } = new ServiceCollection();
        IServer? IWebApplicationBuilder<TelegramWebApplication>.Server => _webApplicationBuilderImplementation.Server;

        public IServer? Server { get; protected set; }


        public void SetServer(IServer server)
        {
            Server = server;
        }
        

        public TelegramWebApplication Build()
        {
            // Добавляем ServiceProvider
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            Services.AddSingleton<IServiceProvider>(serviceProvider);

            if (Server is null)
            {
                throw new Exception($"Не настроен {nameof(Server)}, необходимо вызвать {nameof(SetServer)}.");
            }

            TelegramWebApplication application = new TelegramWebApplication(Server, Services.BuildServiceProvider());

            Services.Clear();
            Server = null;

            return application;
        }
    }
}