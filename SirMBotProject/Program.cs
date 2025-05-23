using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using SirMBotProject.Services;

namespace SirMBotProject
{
    public class Program
    {

        public static async Task Main()
        {
            Env.Load(@"..\..\..\.env");
            var token = System.Environment.GetEnvironmentVariable("BOT_TOKEN");

            var services = ConfigureServices();

            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await services.GetRequiredService<StartupService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<StartupService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<InteractionService>(provider =>
                new InteractionService(provider.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
