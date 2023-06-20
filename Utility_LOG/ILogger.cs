using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility_LOG
{
    public interface ILogger
    {
        void Init();
        void LogEvent(string msg);
        void LogError(string msg);
        void LogWarning(string msg);
        void LogException(string msg, Exception exce);
        void LogCheckHouseKeeping();
    }
}
