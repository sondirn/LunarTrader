using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpaca.Markets;
using LunarTrader.Enums;

namespace LunarTrader.Interfaces
{
    public interface ILoggerService
    {
        static ILoggerService Instance { get; }
        List<LogLevel> LogLevels { get; }

        public void LogTrace(object message);
        public void LogDebug(object message);
        public void LogInfo(object message);
        public void LogWarn(object message);
        public void LogError(object message);
        public void LogFatal(object message);
    }
}
