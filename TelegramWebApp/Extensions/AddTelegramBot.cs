using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace TelegramWebApp.Extensions
{
    public static class AddTelegramBotExtension
    {
        public static void AddTelegramBot(this TelegramWebApplicationBuilder builder, Func<string> token)
        {
            ITelegramBotClient bot = new TelegramBotClient(token.Invoke());
            
            builder.Services.AddSingleton<ITelegramBotClient>(bot);
        }
    }
}