using System;
using System.Threading.Tasks;
using Alpaca.Markets;

namespace LunarTrader.Interfaces
{
    public interface IDateTimeService
    {
        ILoggerService LoggerService { get; }
        TimeZoneInfo LocalTimeZone { get; }
        TimeZoneInfo UtcTimeZone { get; }
        public DateTime UtcToLocal(DateTime time);
        public DateTime LocalToUtc(DateTime time);
    }
}