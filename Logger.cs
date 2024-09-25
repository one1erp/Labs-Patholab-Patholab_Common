using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Patholab_Common
{


    public static class Logger
    {
        public static void WriteLogFile(Exception exception)
        {
            WriteExceptionToLog(exception.ToString());
            return;
            try
            {

                var fullPath = GetFullPath("LogPath", "Log");


                using (FileStream file = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(DateTime.Now);
                    streamWriter.WriteLine("Message");
                    streamWriter.WriteLine(exception.Message);
                    streamWriter.WriteLine("InnerException");
                    streamWriter.WriteLine(exception.InnerException);
                    streamWriter.WriteLine("StackTrace");
                    streamWriter.WriteLine(exception.StackTrace);
                    if (exception.InnerException != null)
                    {
                        streamWriter.WriteLine("InnerException.Message");
                        streamWriter.WriteLine(exception.InnerException.Message);
                    }
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("///////////////////////////////////////////");
                    streamWriter.WriteLine();
                    streamWriter.Close();
                }
            }
            catch
            {
            }


        }

        public static void WriteLogFile(string query)
        {
            WriteExceptionToLog(query);
            return;
            try
            {

                var fullPath = GetFullPath("LogPath", "Log");

                using (FileStream file = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"));

                    streamWriter.WriteLine(query);
                    streamWriter.WriteLine();
                    streamWriter.Close();
                }
            }
            catch
            {
            }
           
        }

        private static string GetFullPath(string keyName, string fileName)
        {
            try
            {

                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = assemblyPath + ".config";
                Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                var appSettings = cfg.AppSettings;

                //Ashi 13/10/20 For citrix envirionment Create folder by user name
                //string path = Path.Combine(appSettings.Settings[keyName].Value, Environment.MachineName);
                string path = Path.Combine(appSettings.Settings[keyName].Value, Environment.UserName.MakeSafeFilename('_'));
                string logFile = fileName + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fullPath = Path.Combine(path, logFile);
                return fullPath;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static void WriteQueries(string query)
        {
            try
            {

                var fullPath = GetFullPath("QueriesPath", "Queries");
                //     var fullPath = @"C:\Queries\logq.txt";// GetFullPath("LogPath", "Log");

                using (FileStream file = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"));

                    streamWriter.WriteLine(query);
                    streamWriter.WriteLine();
                    streamWriter.Close();
                }
            }
            catch
            {
            }


        }

        public static void MyLog(string sss)
        {
            try
            {



                using (FileStream file = new FileStream("C:\\log.txt", FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"));

                    streamWriter.WriteLine(sss);
                    streamWriter.WriteLine();
                    streamWriter.Close();
                }
            }
            catch
            {
            }


        }

        public static void WriteEventVieweer()
        {
            string sSource;
            string sLog;
            string sEvent;

            sSource = "Patholab";
            sLog = "Application";
            sEvent = "Sample Event";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, sEvent);
            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Warning, 234);
        }



        public static void WriteXml(MSXML.DOMDocument objDoc, bool p)
        {
            var fullPath = GetFullPath("XmlPath", "Queries");

            using (FileStream file = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
            {
                var streamWriter = new StreamWriter(file);
                streamWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                streamWriter.WriteLine();
                streamWriter.Close();
            }
        }


        #region AVIGAIL 01/08/24

        private static ThreadLocal<string> logPath = new ThreadLocal<string>(() => string.Empty);
        private static ThreadLocal<bool> isLoggerDefined = new ThreadLocal<bool>(() => false);
        private static ThreadLocal<bool> isInfoLoggingEnabled = new ThreadLocal<bool>(() => false);
        private static ThreadLocal<string> fullLogPath = new ThreadLocal<string>(() => string.Empty);
        private static StackFrame frame;

        internal static void DefineLogger()
        {
            if (isLoggerDefined.Value) return;

            try
            {
                // Try to load configuration from the main application's config file
                Configuration cfg = null;
                string configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                try
                {
                    ExeConfigurationFileMap map = new ExeConfigurationFileMap
                    {
                        ExeConfigFilename = configFilePath
                    };
                    cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                }
                catch
                {
                    // Handle the case where the main application's config file cannot be loaded
                }

                // If main application's config file was not loaded, try loading the config file for the current application (patholab_common config)
                if (cfg == null || cfg.AppSettings.Settings.Count == 0)
                {
                    string assemblyPath = Assembly.GetExecutingAssembly().Location;
                    try
                    {
                        ExeConfigurationFileMap map = new ExeConfigurationFileMap
                        {
                            ExeConfigFilename = assemblyPath + ".config"
                        };
                        cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                    }
                    catch
                    {
                        // Handle the case where the current assembly's config file cannot be loaded
                    }
                }

                // If still no configuration, use default settings
                var appSettings = cfg?.AppSettings ?? new AppSettingsSection();
                string logPathFromConfig = appSettings.Settings["LogPath"]?.Value ?? string.Empty;
                isInfoLoggingEnabled.Value = appSettings.Settings["EnableInfoLogFlag"]?.Value != "F";

                // Define default log path
                string defaultLogFolder = @"C:\temp\";
                string safeUserName = Environment.UserName.MakeSafeFilename('_');
                string logFolderPath = string.IsNullOrEmpty(logPathFromConfig) ? Path.Combine(defaultLogFolder, safeUserName) : Path.Combine(logPathFromConfig, safeUserName);

                // Define log file name with current date
                string logFileName = $"Log-{DateTime.Now:dd-MM-yyyy}.txt";

                // Set the full log path
                fullLogPath.Value = Path.Combine(logFolderPath, logFileName);
                logPath.Value = fullLogPath.Value; // Initialize logPath for use in WriteToLog

                // If logging is enabled, ensure the directory exists
                if (!Directory.Exists(logFolderPath))
                {
                    Directory.CreateDirectory(logFolderPath);
                }

                isLoggerDefined.Value = true;
            }
            catch (Exception ex)
            {
                // Handle the exception, such as logging it
                Console.WriteLine($"Error in DefineLogger method: {ex.Message}");
            }
        }

        internal static string GetCallingMethodDetails()
        {
            string methodName = "UnknownMethod";
            string className = "UnknownClass";
            string namespaceName = "UnknownNamespace";

            try
            {
                var stackTrace = new StackTrace();                
                var frame = stackTrace.GetFrame(3)?.GetMethod().Name == "WriteLogFile" ? stackTrace.GetFrame(4) : stackTrace.GetFrame(3);
                var callingMethod = frame?.GetMethod();
                methodName = callingMethod?.Name ?? "UnknownMethod";
                className = callingMethod?.DeclaringType?.Name ?? "UnknownClass";
                namespaceName = callingMethod?.DeclaringType?.Namespace ?? "UnknownNamespace";
            }
            catch (Exception)
            {
                // Ignore exceptions from stack trace retrieval
            }
            return $"{DateTime.Now} [{namespaceName}.{className}.{methodName}]";
        }

        //Logs are only written if the EnableInfoLogFlag is set to "T" in the configuration
        public static void WriteInfoToLog(string strLog)
        {
            DefineLogger();
            if (!isInfoLoggingEnabled.Value) return;

            WriteLog(strLog, "Info");
        }

        //Logs are always written
        public static void WriteExceptionToLog(string strLog)
        {
            DefineLogger();
            WriteLog(strLog, "Exception");
        }

        public static void WriteExceptionToLog(Exception ex)
        {
            DefineLogger();
            WriteLog(ex.ToString(), "Exception");
        }

        private static void WriteLog(string strLog, string logType)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logPath.Value, true))
                {
                    string callingMethodDetails = GetCallingMethodDetails();
                    sw.WriteLine($"{callingMethodDetails} {logType}"); // Log method details
                    sw.WriteLine($"{strLog}\n");
                }
            }
            catch (Exception ex)
            {
                // Consider logging this exception to a file or other medium
                Console.WriteLine($"Error writing to log: {ex.Message}");
            }
        }

        #endregion
    }


}
