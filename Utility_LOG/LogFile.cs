﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utility_LOG;

public class LogFile : ILogger
{
    private string _fileName;
    private int count = 0;
    //private const int MaxFileSize = 50 * 1024 * 1024; //50MB
    private const int MaxFileSize = 50 * 1024; //50MB
    private int logWritesSinceHouseKeeping = 0;
    private const int WritesBeforeHouseKeeping = 50;


    private StreamWriter logStreamWriter;

    public LogFile()
    {
        Init();
    }

    public string FileName
    {
        get { return _fileName; }
        private set { _fileName = value; }
    }

	//public void Init()
	//{
	//    FileName = $"{DateTime.Now:dd-MM-yyyy}_Log{count}.txt";

	//    if (logStreamWriter != null)
	//    {
	//        logStreamWriter.Dispose();
	//        logStreamWriter = null;
	//    }

	//    try
	//    {
	//        logStreamWriter = new StreamWriter(FileName, true);
	//    }
	//    catch (IOException ex)
	//    {
	//        Console.WriteLine($"Cannot open file '{FileName}' for writing: {ex.Message}");
	//        throw; // Or handle the error as appropriate
	//    }
	//}

	public void Init()
	{
		FileName = $"{DateTime.Now:dd-MM-yyyy}_Log{count}.txt";

		if (logStreamWriter != null)
		{
			logStreamWriter.Dispose();
			logStreamWriter = null;
		}

		const int maxRetries = 5;
		int retryCount = 0;
		bool fileOpened = false;

		while (!fileOpened && retryCount < maxRetries)
		{
			try
			{
				logStreamWriter = new StreamWriter(FileName, true);
				fileOpened = true;
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Cannot open file '{FileName}' for writing: {ex.Message}");
				retryCount++;
				Thread.Sleep(1000); // Wait for 1 second before retrying
			}
		}

		if (!fileOpened)
		{
			Console.WriteLine($"Unable to open the file '{FileName}' after {maxRetries} attempts. Aborting.");
			//throw new IOException("Failed to open file after multiple attempts.");
		}
	}



	public void LogEvent(string msg)
    {
        WriteLog($"[EVENT][{DateTime.Now}] {msg}");
    }

    public void LogError(string msg)
    {
        WriteLog($"[ERROR][{DateTime.Now}] {msg}");
    }

    public void LogException(string msg, Exception exce)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[EXCEPTION][{DateTime.Now}] {msg}, {exce.Message}");
        sb.AppendLine("Stack Trace: " + exce.StackTrace);
        WriteLog(sb.ToString());
    }

    private void WriteLog(string msg)
    {
        try
        {
            logStreamWriter.WriteLine(msg);
            logStreamWriter.Flush();
            logWritesSinceHouseKeeping++;

            if (logWritesSinceHouseKeeping >= WritesBeforeHouseKeeping)
            {
                LogCheckHouseKeeping();
                logWritesSinceHouseKeeping = 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error - WriteLog Func: {ex.Message}");
        }
    }

    public void LogCheckHouseKeeping()
    {
        var logFileInfo = new FileInfo(FileName);
        logFileInfo.Refresh();
        if (logFileInfo.Length <= MaxFileSize) // check size in bytes
            return;

        // Close the current log file
        logStreamWriter.Close();
        logStreamWriter.Dispose();

        // Compress and rename the current log file
        var archivePath = Path.Combine(logFileInfo.DirectoryName, "archive");
        Directory.CreateDirectory(archivePath);

        // Determine the archive file based on the month and year
        var archiveFileName = $"{DateTime.Now:yyyy-MM}.zip";
        var archiveFilePath = Path.Combine(archivePath, archiveFileName);

        // Move the log file to the archive directory temporarily
        var tempFilePath = Path.Combine(archivePath, logFileInfo.Name);
        File.Move(logFileInfo.FullName, tempFilePath);

        try
        {
            // Add the log file to the archive
            using (FileStream zipToOpen = new FileStream(archiveFilePath, FileMode.OpenOrCreate))
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
            {
                var entryName = Path.GetFileName(tempFilePath);
                var entry = archive.GetEntry(entryName);
                var entryVersion = 0;
                while (entry != null)
                {
                    // File with the same name already exists in the ZIP, append a version number
                    entryName = Path.GetFileNameWithoutExtension(tempFilePath) + $"_{++entryVersion}" + Path.GetExtension(tempFilePath);
                    entry = archive.GetEntry(entryName);
                }
                // Add the file to the existing ZIP
                archive.CreateEntryFromFile(tempFilePath, entryName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"There was a problem with 'LogCheckHouseKeeping' function: {e.Message}");
            throw;
        }
        finally
        {
            // Delete the original log file from the archive directory
            File.Delete(tempFilePath);

            // Create a new log file and reset the counter
            count++;
            Init();
        }
    }


    public void Dispose()
    {
        logStreamWriter?.Dispose();
        logStreamWriter = null;
    }
}

