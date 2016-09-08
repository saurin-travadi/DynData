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

        public static bool LogError(string message)
        {
            return LogToFile(message, "Error");
        }

        public static bool LogInfo(string message)
        {
            return LogToFile(message, "Info");
        }

        private static bool LogToFile(string Message, string mode)
        {
            TextWriter tw = null;
            bool retVal = true;
            try
            {
                // for thread safe write, we are using Synchronization
                var filePath = System.IO.Path.Combine(DirectoryPath, string.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd"))); 
                tw = TextWriter.Synchronized(File.AppendText(filePath));
                tw.WriteLine(string.Format("{0} - {1} - {2}", DateTime.Now.ToString(), mode, Message));
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            if (tw != null)
            {
                tw.Flush();
                tw.Close();
                tw = null;
            }
            return retVal;
        }

        private static bool CreateDirectory()
        {

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            return true;
        }

    }
}
