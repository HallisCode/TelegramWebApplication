using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure;
using SimpleNetFramework.Infrastructure.Server;
using Telegram.Bot.Types;

namespace TelegramWebApp
{
    public class TelegramWebApplication : WebApplicationBase<Update>, ITelegramWebApplication
    {
        public TelegramWebApplication(IServer server, IServiceProvider provider) : base(server, provider)
        {
        }

        protected override Task HandleServerRequest(IServerRequest request)
        {
            request.Response = GetResponse200();

            if (_middlewares.Count > 0)
            {
                string _encodedBody = Encoding.UTF8.GetString(request.Request.Body);
                Update? update = JsonSerializer.Deserialize<Update>(_encodedBody);

                if (update.Id == 0)
                {
                    return Task.CompletedTask;
                }

                _middlewares[0].Invoke(update);
            }

            return Task.CompletedTask;
        }

        protected virtual IHttpResponse GetResponse200()
        {
            HttpResponse response = new HttpResponse(
                statusCode: 200,
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