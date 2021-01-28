using System.Collections.Generic;
using System.Threading.Tasks;
using Alpaca.Markets;
using LunarTrader.Utils;

namespace LunarTrader.Interfaces
{
    public interface ICoreService
    {
        Settings Config { get; }
        IAlpacaTradingClient TradingClient { get; }
        IAlpacaStreamingClient StreamingClient { get; }
        ILoggerService LoggerService { get; }
        IAccountService AccountService { get; }
        List<Task> RunningTasks { get; }
        ICommandService CommandService { get; }

        Task Run();
        void SubmitTask(Task task);
    }
}