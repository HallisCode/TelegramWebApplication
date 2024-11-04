using System;
using Microsoft.Extensions.DependencyInjection;
using TeleRoute.Core.Routing;
using TeleRoute.Infrastructure.Routing;

namespace TelegramWebApp.Extensions
{
    public static class AddTeleRouteExtension
    {
        /// <summary>
        /// Добавляет необходимые сервисы для работы TeleRouting.
        /// </summary>
        /// <param name="config">Конфигрурирование маршрутов.</param>
        public static void AddTeleRouting(this TelegramWebApplicationBuilder builder, Action<IRouteBuilder> config)
        {
            // Добавляем древо маршрутизации
            IRouteBuilder routeBuilder = new RouteBuilder();
            config.Invoke(routeBuilder);

            IRouteTree routeTree = routeBuilder.Build();
            builder.Services.AddSingleton<IRouteTree>(routeTree);

            // Добавляем обработчик, сопоставляющий запросм с маршрутом
            builder.Services.AddSingleton<IRouteHandler, RouteHandler>();
        }

        /// <summary>
        /// Добавлять в последнню очередь в конвеер Middlewares. Сопоставляет запрос с маршрутом и вызывает обработчик.
        /// Не вызывает последующие миддлвари.
        /// </summary>
        public static void UseTeleRouting(this TelegramWebApplication app)
        {
            app.UseMiddleware<RoutingMiddleware>();
        }
    }
}