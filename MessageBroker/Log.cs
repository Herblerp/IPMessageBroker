using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessageBroker
{
    public class Log
    {

        private static string _logTitle;
        private static bool _showDebug;

        private static Log _instance;
        private static readonly object padlock = new object();

        private Log() { }

        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _logTitle = "Log_" + DateTime.Now.ToString("MM-dd-yy_H-mm-ss") + ".txt";
                            _instance = new Log();

                            if (!Directory.Exists("Logs"))
                            {
                                Directory.CreateDirectory("Logs");
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        public void Welcome()
        {
            string message = @"
  __  __                                  ____            _              
 |  \/  | ___  ___ ___  __ _  __ _  ___  | __ ) _ __ ___ | | _____ _ __  
 | |\/| |/ _ \/ __/ __|/ _` |/ _` |/ _ \ |  _ \| '__/ _ \| |/ / _ \ '__| 
 | |  | |  __/\__ \__ \ (_| | (_| |  __/ | |_) | | | (_) |   <  __/ |    
 |_|  |_|\___||___/___/\__,_|\__, |\___| |____/|_|  \___/|_|\_\___|_|    
                             |___/   

";
            Console.WriteLine(message);
        }

        public void SetLogFileTitle(string title)
        {
            _logTitle = title + "_" + DateTime.Now.ToString("MM-dd-yy_H-mm-ss") + ".txt";
        }

        public void ShowDebugMessages(bool showdebug)
        {
            _showDebug = showdebug;
        }

        public void LogMessage(string message, string severity)
        {
            lock (padlock)
            {
                using (StreamWriter _writer = File.AppendText("Logs/" + _logTitle))
                {
                    DateTime time = DateTime.Now;
                    string stime = time.ToString("hh:mm:ss.ff");

                    if (severity == "info")
                    {
                        string log = "[" + stime + "] INFO: " + message;

                        _writer.WriteLine(log);
                        Console.WriteLine(log);
                    }
                    else if (severity == "warning")
                    {
                        string log = "[" + stime + "] WARNING: " + message;

                        _writer.WriteLine(log);
                        Console.WriteLine(log);
                    }
                    else if (severity == "error")
                    {
                        string log = "[" + stime + "] ERROR: " + message;

                        _writer.WriteLine(log);
                        Console.WriteLine(log);
                    }
                    else if (severity == "debug" && _showDebug)
                    {
                        string log = "[" + stime + "] DEBUG: " + message;

                        _writer.WriteLine(log);
                        Console.WriteLine(log);
                    }
                    else if (severity != "debug")
                    {
                        string log = "[" + stime + "] ?????: " + message;

                        _writer.WriteLine(log);
                        Console.WriteLine(log);
                    }
                }
            }
        }

        public void LogMessageType(string message, string type)
        {
            DateTime time = DateTime.Now;
            string stime = time.ToString("hh:mm:ss.ff");
            string log = "[" + stime + "] INFO: " + message;

            Console.Write(log);
            Console.WriteLine(type.ToUpper());
        }
    }
}
