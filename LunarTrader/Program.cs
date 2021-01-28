using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.DependencyInjection;
using LunarTrader.Interfaces;
using LunarTrader.Services;
using LunarTrader.Utils;
using CommandService = LunarTrader.Services.CommandService;

namespace LunarTrader
{
    public static class Core
    {
        private static Settings _settings;
        public static CommandService CommandService;
        public static IServiceProvider ServiceProvider;
        public static ILoggerService Logger;
        
        public static async Task Main(string[] args)
        {
            //Load Settings
            var loader = new FileLoader();
            _settings = loader.ReadSettings();
            try
            {
                ServiceProvider = buildServiceProvider().Result;
                var core = ServiceProvider.GetService<ICoreService>();
                await core.Run();
            }
            catch (Exception e)
            {
                var logger = new LoggerService(_settings);
                logger.LogFatal($"Initialization failed with exception: {e}");
                Console.ReadKey();
            }

            Console.ReadKey();
        }

        private static async Task<IServiceProvider> buildServiceProvider()
        {
            var client = 
                Environments.Paper.GetAlpacaTradingClient(new SecretKey(_settings.KeyId, _settings.SecretKey));
            var streamingClient =
                Environments.Paper.GetAlpacaStreamingClient(new SecretKey(_settings.KeyId, _settings.SecretKey));
            Logger = new LoggerService(_settings);
            CommandService = new CommandService(Logger);
            
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(_settings);
            services.AddSingleton<ILoggerService>(Logger);
            services.AddSingleton<ICommandService>(CommandService);
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton(client);
            services.AddSingleton(streamingClient);
            services.AddSingleton<IClock>(await client.GetClockAsync());
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<ICoreService, CoreService>();
            
            return services.BuildServiceProvider();
        }
        
    }
}