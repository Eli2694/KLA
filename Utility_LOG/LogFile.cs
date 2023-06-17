using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Utility_LOG
{
    public class LogFile: ILogger
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private static int count = 0;
        private static int MaxFileSize = 45 * 1024 * 1024; //45MB

        public void Init() 
        {
			FileName = $"{DateTime.Now.ToString("dd-MM-yyyy")}_Log{count}.txt";
            LogCheckHouseKeeping();
			FileInfo CheckFile = new FileInfo(FileName);
			if (!CheckFile.Exists)
            {
                try
                {				
					using (File.Create(FileName)) { }
				}
                catch (Exception ex)
                {
					Console.WriteLine($"Error creating file: {ex.Message}");
                }
			}
           
        }
        public void LogEvent(string msg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("[EVENT][" + DateTime.Now + "] " + msg);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(" LogEvent Func: " + ex.Message);
            }
        }
        public void LogError(string msg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("[ERROR][" + DateTime.Now + "] " + msg);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(" LogError Func: " + ex.Message);
            }
        }
        public void LogException(string msg, Exception exce)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("[EXCEPTION][" + DateTime.Now + "] " + msg + ", " + exce.Message);
                    sw.WriteLine("Stack Trace: " + exce.StackTrace);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(" LogException Func: " + ex.Message);
            }
        }
        public void LogCheckHouseKeeping()
        {
            FileInfo file = new FileInfo(FileName);
            try
            {
                if (file.Exists && file.Length > MaxFileSize) // check size in bytes
                {
                    count++;
                    Init();
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine($"there was a problem with 'LogCheckHouseKeeping' function: {e.Message}");
                throw;
            }

        }
    }
}


