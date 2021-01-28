﻿using System.Collections.Generic;
using TestTradingBot.Enums;

namespace TestTradingBot.Utils
{
    public class Settings
    {
        public string KeyId;
        public string SecretKey;
        public List<LogLevel> LogLevels = new List<LogLevel> {LogLevel.Trace, LogLevel.Info, LogLevel.Debug,LogLevel.Warn, LogLevel.Error, LogLevel.Fatal};

        public override string ToString()
        {
            return $"KeyId: {KeyId} \nSecretKey: {SecretKey}";
        }
    }
}