using System;
using System.Collections.Generic;
using System.Drawing;
using Alpaca.Markets;
using Pastel;
using LunarTrader.Enums;
using LunarTrader.Interfaces;
using LunarTrader.Utils;

namespace LunarTrader.Services
{
    public class LoggerService : ILoggerService, IDisposable
    {
        public List<LogLevel> LogLevels { get; }

        public static ILoggerService Instance { get; }

        public LoggerService(Settings settings)
        {
            LogLevels = settings.LogLevels;
            LogInfo("Setting up Logger");
        }
        
        public void LogTrace(object message)
        {
            if (!LogLevels.Contains(LogLevel.Trace)) return;
            
            var color = Color.FromArgb(123, 124, 122);
            var consoleMessage = $"[{DateTime.Now}] [Level: {LogLevel.Trace}] {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        public void LogDebug(object message)
        {
            if (!LogLevels.Contains(LogLevel.Debug)) return;
            
            var color = Color.FromArgb(255, 152, 0);
            var consoleMessage = $"{DateTime.Now}: {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        public void LogInfo(object message)
        {
            if (!LogLevels.Contains(LogLevel.Info)) return;
            
            var color = Color.FromArgb(104, 195, 85);
            var consoleMessage = $"{DateTime.Now}: {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        public void LogWarn(object message)
        {
            if (!LogLevels.Contains(LogLevel.Warn)) return;
            
            var color = Color.FromArgb(241, 210, 46);
            var consoleMessage = $"{DateTime.Now}: {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        public void LogError(object message)
        {
            if (!LogLevels.Contains(LogLevel.Error)) return;
            
            var color = Color.FromArgb(233, 53, 25);
            var consoleMessage = $"{DateTime.Now}: {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        public void LogFatal(object message)
        {
            if (!LogLevels.Contains(LogLevel.Fatal)) return;
            
            var color = Color.FromArgb(158, 45, 34);
            var consoleMessage = $"{DateTime.Now}: {message}".Pastel(color);
            Console.WriteLine(consoleMessage);
        }

        private void ResetCurrentCommand()
        {
            var cmdService = Core.CommandService;
            cmdService.CurrentCommand = "";
        }
        
        public void Dispose()
        {
        }
    }
}