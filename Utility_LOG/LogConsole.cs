using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility_LOG
{
    public class LogConsole: ILogger
    {
        public void Init()
        {
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[INFO][" + DateTime.Now + "] " + message);
            Console.ResetColor();
        }

        public void LogEvent(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[EVENT][" + DateTime.Now + "] " + message + "\n");
            Console.ResetColor();

        }

        public void LogError(string message)
        {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[ERROR][" + DateTime.Now + "] " + message + "\n");
			Console.ResetColor();
		}


        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARNING][" + DateTime.Now + "] " + message + "\n");
            Console.ResetColor();
        }

        public void LogException(string message, Exception ex)
        {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[EXCEPTION][" + DateTime.Now + "] " + message + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
			Console.ResetColor();
		}

        public void LogCheckHouseKeeping()
        {
        }
    }
}
