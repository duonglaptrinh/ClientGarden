using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Client.Extension;
using UnityEngine;

namespace Game.Log
{
    public static class UnityLogCustom
    {
        private static bool isInitialized;
        public static string LogPath { get; private set; }

        private static bool EnableWriteLog { get; set; } = true;

        private static List<Log> LogQueue { get; } = new List<Log>();

        private static readonly object lockObject = new object();

        private static DateTime lastLogTime;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (isInitialized)
            { 
                return;
            }

            isInitialized = true;

#if UNITY_EDITOR
            LogPath = $"{Application.dataPath}/../";
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            LogPath = $"{Application.dataPath}/../";
#elif UNITY_STANDALONE_OSX
            LogPath = $"{Application.dataPath}/../../";
#else
            LogPath = $"{Application.persistentDataPath}/";
#endif
            var folder = Path.Combine(LogPath, "Logs");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            LogPath = Path.Combine(folder, $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            lastLogTime = DateTime.Now;
            Application.logMessageReceived += ApplicationOnLogMessageReceived;

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    lock (lockObject)
                    {
                        foreach (var log in LogQueue)
                        {
                            if(lastLogTime.Day != DateTime.Now.Day)
                            {
                                LogPath = Path.Combine(folder, $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                                lastLogTime = DateTime.Now;
                            }
                                
                            WriteLog(LogPath, log);
                        }
                        
                        LogQueue.Clear();
                    }
                }
            });

            DebugExtension.Log($"[UnityLogCustom] Initialized log file path: {LogPath}");

        }

        private static void ApplicationOnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            lock (lockObject)
            {
                var log = new Log()
                {
                    condition = condition,
                    stacktrace = stacktrace,
                    type = type,
                    dateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                };
                LogQueue.Add(log);
            }
        }

        private static void WriteLog(string path, Log log)
        {
            if (!EnableWriteLog) return;

            void Write(StreamWriter sw)
            {
                sw.Write($"[{log.dateTime}]");
                sw.Write($"[{log.type}]");
                sw.WriteLine(log.condition);
                if (log.type == LogType.Error || log.type == LogType.Exception)
                    sw.WriteLine(log.stacktrace);
            }
            
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (var sw = File.CreateText(path))
                {
                    Write(sw);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    Write(sw);
                }
            }
        }

        public static void DebugLog(string log)
        {
            DebugExtension.Log(log);
        }

        public static void DebugErrorLog(string log)
        {
            DebugExtension.LogError(log);
        }
    }

    internal struct Log
    {
        public string condition;
        public string stacktrace;
        public LogType type;

        public string dateTime;
    }
}