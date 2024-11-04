using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure;
using SimpleNetFramework.Infrastructure.Server;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramWebApp
{
    public class TelegramWebApplication : WebApplicationBase<Update>, ITelegramWebApplication
    {
        public TelegramWebApplication(
            IServer server,
            IServiceProvider provider,
            ILogger logger,
            IConfigurationRoot configuration) : base(
            server: server,
            provider: provider,
            configuration: configuration,
            logger: logger)
        {
        }

        protected override async Task HandleServerRequest(IServerRequest request)
        {
            request.Response = GetResponse200();

            if (_middlewares.Count > 0)
            {
                string _encodedBody = Encoding.UTF8.GetString(request.Request.Body);
                Update? update = JsonSerializer.Deserialize<Update>(_encodedBody, JsonBotAPI.Options);

                if (update.Id == 0)
                {
                    return;
                }

                await _middlewares[0].Invoke(update);
            }
        }

        protected virtual IHttpResponse GetResponse200()
        {
            HttpResponse response = new HttpResponse(
                statusCode: HttpStatusCode.NoContent,
                message: "OK",
                body: null,
                protocol: "HTTP/1.1"
            );

            response.Headers.Add("Connection", "close");
            response.Headers.Add("Content-type", "application/json");

            return response;
        }
    }
}