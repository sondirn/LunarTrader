using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarTrader.Enums
{
    public enum LogLevel
    {
        Off,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public enum LogType
    {
        Console,
        LogFile,
        Both
    }
}
