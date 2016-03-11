using System;
using System.Text;

namespace LogonTimes.Logging
{
    public interface ILogger
    {
        void AddLineToMessage(StringBuilder stringBuilder, string message);
        void Dispose();
        void Log(string message, DebugLevels debugLevel);
        void LogException(string header, DebugLevels debugLevel, Exception ex);
        bool ShouldLog(DebugLevels debugLevel);
    }
}