using System.Threading.Tasks;
using SimpleNetFramework.Core.Middleware;
using Telegram.Bot.Types;
using TeleRoute.Core.Routing;

namespace TelegramWebApp
{
    /// <summary>
    /// Вызывает контроллер для обработки текущего update. Не вызыает последующий обработчик middleware.
    /// </summary>
    public class RoutingMiddleware : IMiddleware<Update>
    {
        private readonly IRouteHandler _routeHandler;
        private MiddlewareDelegate<Update> _next;


        public MiddlewareDelegate<Update> Next
        {
            set => _next = value;
        }


        public RoutingMiddleware(IRouteHandler routeHandler)
        {
            _routeHandler = routeHandler;
        }

        public async Task Invoke(Update update)
        {
            await _routeHandler.Handle(update);
        }
    }
}