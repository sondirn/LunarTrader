using System;
using System.Threading.Tasks;
using Alpaca.Markets;
using TestTradingBot.Interfaces;

namespace TestTradingBot.Utils
{
    public class DateTimeService : IDateTimeService
    {
        public ILoggerService LoggerService { get; }
        public TimeZoneInfo LocalTimeZone { get; }
        public TimeZoneInfo UtcTimeZone { get; }
        
        public DateTimeService(ILoggerService loggerService)
        {
            LoggerService = loggerService;
            LoggerService.LogInfo("Setting up Date Time Service...");
            LocalTimeZone = TimeZoneInfo.Local;
            UtcTimeZone = TimeZoneInfo.Utc;
        }

        public DateTime UtcToLocal(DateTime time)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(time, LocalTimeZone);
        }

        public DateTime LocalToUtc(DateTime time)
        {
            return TimeZoneInfo.ConvertTimeToUtc(time);
        }
    }
}