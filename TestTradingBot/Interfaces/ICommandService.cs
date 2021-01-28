using System;
using System.Collections.Generic;
using System.Reflection;
using TestTradingBot.Services;
using WebSocket4Net.Common;

namespace TestTradingBot.Interfaces
{
    public interface ICommandService
    {
        Dictionary<string, CommandInfo> Commands { get; }
        Dictionary<string, Type> Enums { get; }
        ILoggerService LoggerService { get; }
        List<string> Sorted { get; }
        List<string> CommandHistory { get; }
        public void Listen();
    }
}