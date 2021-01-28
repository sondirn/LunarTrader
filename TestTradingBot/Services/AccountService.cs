using System;
using Alpaca.Markets;
using Microsoft.Extensions.DependencyInjection;
using TestTradingBot.Attributes;
using TestTradingBot.Interfaces;

namespace TestTradingBot.Services
{
    public class AccountService : IAccountService
    {
        public AccountService(ILoggerService logger, IAlpacaTradingClient tradingClient)
        {
            Logger = logger;
            TradingClient = tradingClient;
            Logger.LogInfo("Fetching Account Data");
            Account = TradingClient.GetAccountAsync().Result;
            if (Account != null)
                Logger.LogInfo("Account Data Fetched");
            else
                throw new Exception("Could not fetch account data");
        }

        public IAlpacaTradingClient TradingClient { get; }
        public IAccount Account { get; }
        public ILoggerService Logger { get; }

        [Command("getAccountDetails", "Gets various details about the trading account")]
        private static void getAccountDetails()
        {
            var account = Core.ServiceProvider.GetService<IAlpacaTradingClient>().GetAccountAsync().Result;

            var result = $"\n" +
                         $"Account Details: \n" +
                         $"Account Number: {account.AccountNumber} \n" +
                         $"Buying Power: {account.BuyingPower} \n" +
                         $"Day Trade Buying Power: {account.DayTradingBuyingPower}\n" +
                         $"Day Trade Count: {account.DayTradeCount}\n" +
                         $"Is Day Trader: {account.IsDayPatternTrader} \n" +
                         $"Equity: {account.Equity} \n";
            Core.Logger.LogInfo(result);
        }
    }
}