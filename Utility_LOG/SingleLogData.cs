using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility_LOG
{

    public enum LogProviderType
    {
        Console,
        File
    }

    public enum LogEventType
    {
        Info,
        Event,
        Error,
        Warning,
        Exception
    }
    public class SingleLogData
    {
        public LogEventType EventType { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public LogProviderType ProviderType { get; set; }

        public SingleLogData(LogEventType eventType, string message, Exception exception, LogProviderType providerType)
        {
            EventType = eventType;
            Message = message;
            Exception = exception;
            ProviderType = providerType;
        }

        public SingleLogData(LogEventType eventType, string message, LogProviderType providerType)
        {
            EventType = eventType;
            Message = message;
            ProviderType = providerType;
        }
    }

}
