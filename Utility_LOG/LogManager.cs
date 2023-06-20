using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility_LOG
{
	public class LogManager : IDisposable
    {
        
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly Task _dequeueTask;
        //private readonly Task _houseKeepingTask;

        private static System.Collections.Generic.Queue<SingleLogData> LogQueue;
		private LogFile? _fileInstance;
		private LogConsole? _consoleInstance;

		public LogManager()
		{
			GetFileInstance();
			GetConsoleInstance();
			LogQueue = new Queue<SingleLogData>();

            // Start the log processing and housekeeping tasks
            _dequeueTask = Task.Run(DequeueMyLog);
            //_houseKeepingTask = Task.Run(CheckHouseKeeping);
        }
		

		public LogFile GetFileInstance()
		{
			if (_fileInstance == null)
			{
				_fileInstance = new LogFile();
				_fileInstance.Init();
			}

			return _fileInstance;
		}
		public LogConsole GetConsoleInstance()
		{
			if (_consoleInstance == null)
			{
				_consoleInstance = new LogConsole();
			}

			return _consoleInstance;
		}


        //thread for dequeue
        private async Task DequeueMyLog()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (LogQueue.TryDequeue(out SingleLogData item))
                    {
                        ProcessLogItem(item);
                    }
                    else
                    {
                        try
                        {
                            // No items in the queue, so wait a little while before checking again
                            await Task.Delay(1000,_cts.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            // handle the exception if necessary
                        }

                        if (_cts.Token.IsCancellationRequested)
                            break;
                    }
                }

                // Process any remaining items in the queue before exiting
                while (LogQueue.TryDequeue(out SingleLogData item))
                {
                    ProcessLogItem(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing log items in (DequeueMyLog): {ex.Message}");
            }
        }

        //private async Task CheckHouseKeeping()
        //{
        //    while (!_cts.Token.IsCancellationRequested)
        //    {
        //        _fileInstance.LogCheckHouseKeeping();
        //        await Task.Delay(TimeSpan.FromHours(1),_cts.Token);
        //    }
        //}

        // ------------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                // Signal the cancellation token to stop the tasks
                _cts.Cancel();

                // Give the tasks a certain amount of time to finish
                // Task.WaitAll(new[] { _dequeueTask, _houseKeepingTask });
                Task.WaitAll(new[] { _dequeueTask });
            }
            catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TaskCanceledException))
            {
                // All exceptions were TaskCanceledException, which is expected during cancellation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception In Dispose Method In Log - {ex.Message}");
            }
        }


        // -------------------------------------------------------------------------------------

        public void LogInfo(string msg, LogProviderType providerType)
        {
            SingleLogData item = new SingleLogData(LogEventType.Info, msg, providerType);
            LogQueue.Enqueue(item);
        }
        public void LogEvent(string msg, LogProviderType providerType)
        {
            SingleLogData item = new SingleLogData(LogEventType.Event, msg, providerType);
            LogQueue.Enqueue(item);
        }

        public void LogError(string msg, LogProviderType providerType)
        {
            SingleLogData item = new SingleLogData(LogEventType.Error, msg, providerType);
            LogQueue.Enqueue(item);
        }

        public void LogWarning(string msg, LogProviderType providerType)
        {
            SingleLogData item = new SingleLogData(LogEventType.Warning, msg, providerType);
            LogQueue.Enqueue(item);
        }

        public void LogException(string msg, Exception exception, LogProviderType providerType)
        {
            SingleLogData item = new SingleLogData(LogEventType.Exception, msg, exception, providerType);
            LogQueue.Enqueue(item);
        }

        // ---------------------------------------------------------------------------------------

        private void ProcessLogItem(SingleLogData item)
        {
            if (item.ProviderType == LogProviderType.Console)
            {
                switch (item.EventType)
                {
                    case LogEventType.Info:
                        _consoleInstance.LogInfo(item.Message);
                        break;
                    case LogEventType.Event:
                        _consoleInstance.LogEvent(item.Message);
                        break;
                    case LogEventType.Warning:
                        _consoleInstance.LogWarning(item.Message);
                        break;
                    case LogEventType.Error:
                        _consoleInstance.LogError(item.Message);
                        break;
                    case LogEventType.Exception:
                        _consoleInstance.LogException(item.Message, item.Exception);
                        break;
                }
            }
            else if (item.ProviderType == LogProviderType.File)
            {
                switch (item.EventType)
                {
                    case LogEventType.Info:
                        _fileInstance.LogInfo(item.Message);
                        break;
                    case LogEventType.Event:
                        _fileInstance.LogEvent(item.Message);
                        break;
                    case LogEventType.Warning:
                        _fileInstance.LogWarning(item.Message);
                        break;
                    case LogEventType.Error:
                        _fileInstance.LogError(item.Message);
                        break;
                    case LogEventType.Exception:
                        _fileInstance.LogException(item.Message, item.Exception);
                        break;
                }
            }
        }
	}

}
