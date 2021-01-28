using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.DependencyInjection;
using TestTradingBot.Attributes;
using TestTradingBot.Interfaces;
using TestTradingBot.Utils;

namespace TestTradingBot.Services
{
    public class CoreService : ICoreService
    {
        public static CoreService Instance;
        public Settings Config { get; }
        public IAlpacaTradingClient TradingClient { get; }
        public IAlpacaStreamingClient StreamingClient { get; }
        public ILoggerService LoggerService { get; }
        public IAccountService AccountService { get; set; }
        public List<Task> RunningTasks { get; }
        public ICommandService CommandService { get; }


        public CoreService(ILoggerService loggerService, Settings config, IAlpacaTradingClient tradingClient, IAlpacaStreamingClient streamingClient, IAccountService accountService, ICommandService commandService)
        {
            CoreService.Instance = this;
            LoggerService = loggerService;
            LoggerService.LogInfo("Initializing Core Service");
            
            Config = config;
            LoggerService.LogInfo("Fetching Trading Client");
            TradingClient = tradingClient;
            LoggerService.LogInfo("Fetching Streaming Client");
            StreamingClient = streamingClient;
            LoggerService.LogInfo("Initializing Account Service");
            AccountService = accountService;
            CommandService = commandService;
            RunningTasks = new List<Task>();

        }
        
        public async Task Run()
        {
            var clock = await TradingClient.GetClockAsync();
            string openStr;
            if (clock.IsOpen)
            {
                openStr = "Markets are currently open";
            }
            else
            {
                openStr = "Markets are currently closed";
            }
            LoggerService.LogInfo($"Bot is ready...");
            LoggerService.LogInfo($"[{openStr}] Type a command to start.");
            
            CommandService.Listen();

            await Task.WhenAll(RunningTasks);
        }

        public void SubmitTask(Task task)
        { 
            RunningTasks.Add(task);
        }

        
        [Command("submitOrderTest", "testing the submit order function")]
        public static async Task SubmitOrderTest()
        {
            var tradingClient = Core.ServiceProvider.GetService<IAlpacaTradingClient>();
            try
            {
                var oder = await tradingClient.PostOrderAsync(OrderSide.Buy.Limit("AAPL", 50, 200));
                Core.Logger.LogInfo($"Submitted Order For {oder.Symbol} @ {oder.LimitPrice} for {oder.Quantity} shares");
                var oder2 = await tradingClient.PostOrderAsync(OrderSide.Buy.Limit("TSLA", 10, 1000));
                Core.Logger.LogInfo($"Submitted Order For {oder2.Symbol} @ {oder2.LimitPrice} for {oder2.Quantity} shares");
            }
            catch (Exception e)
            {
                Core.Logger.LogWarn($"Could not do order: {e.Message}");
            }
        }
    }
}