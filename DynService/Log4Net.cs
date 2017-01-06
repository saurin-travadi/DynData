using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using Microsoft.VisualBasic;
    using System.IO;
    using System.Configuration;
    using System;
    using System.Data.SqlClient;

    public static class clsLog
    {
        private static string DirectoryPath { get; set; }
        
        static clsLog()
        {
            DirectoryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["Log_File_Path"]);
            CreateDirectory();
        }

        public static void Log(string message)
        {
            LogToFile(message, "Service");
        }

        public static void LogError(string message)
        {
            LogToFile(message, "Error");
        }

        public static void LogInfo(string message)
        {
            if (ConfigurationManager.AppSettings["Log_Type"] == "Info")
                LogToFile(message, "Info");
        }

        private static void LogToFile(string Message, string mode)
        {
            TextWriter tw = null;
            try
            {
                // for thread safe write, we are using Synchronization
                var filePath = System.IO.Path.Combine(DirectoryPath, string.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd"))); 
                tw = TextWriter.Synchronized(File.AppendText(filePath));
                tw.WriteLine(string.Format("{0} - {1} - {2}", DateTime.Now.ToString(), mode, Message));
            }
            catch 
            {
                //ignore this error
            }

            if (tw != null)
            {
                tw.Flush();
                tw.Close();
                tw = null;
            }
        }

        private static bool CreateDirectory()
        {

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            return true;
        }

    }
}
