using Alpaca.Markets;

namespace LunarTrader.Interfaces
{
    public interface IAccountService
    {
        IAlpacaTradingClient TradingClient { get; }
        IAccount Account { get; }
        ILoggerService Logger { get; }
    }
}