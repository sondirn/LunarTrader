using Alpaca.Markets;

namespace TestTradingBot.Interfaces
{
    public interface IAccountService
    {
        IAlpacaTradingClient TradingClient { get; }
        IAccount Account { get; }
        ILoggerService Logger { get; }
    }
}