using System;
using System.Diagnostics;
using System.Reflection;
using UnityModManagerNet;

namespace QuickCast.Utility
{
    internal class Logger
    {
        private UnityModManager.ModEntry.ModLogger _logger;

        public Logger(UnityModManager.ModEntry.ModLogger logger)
        {
            _logger = logger;
        }

        public void Critical(string str)
        {
            _logger.Critical(str);
        }

        public void Critical(object obj)
        {
            _logger.Critical(obj?.ToString() ?? "null");
        }

        public void Error(Exception e)
        {
            _logger.Error($"{e.Message}\n{e.StackTrace}");
            if (e.InnerException != null)
                Error(e.InnerException);
        }

        public void Error(string str)
        {
            _logger.Error($"{str}");
        }

        public void Error(object obj)
        {
            _logger.Error($"{obj?.ToString()}" ?? "null");
        }

        public void Log(string str)
        {
            _logger.Log(str);
        }

        public void Log(object obj)
        {
            _logger.Log(obj?.ToString() ?? "null");
        }

        public void Warning(string str)
        {
            _logger.Warning($"{str}");
        }

        public void Warning(object obj)
        {
            _logger.Warning($"{obj?.ToString()}" ?? "null");
        }

        [Conditional("DEBUG")]
        public void Debug(MethodBase method, params object[] parameters)
        {
            _logger.Log($"{method.DeclaringType.Name}.{method.Name}({string.Join(", ", parameters)})");
        }

        [Conditional("DEBUG")]
        public void Debug(string str)
        {
            _logger.Log($"{str}");
        }

        [Conditional("DEBUG")]
        public void Debug(object obj)
        {
            _logger.Log($"{obj?.ToString()}" ?? "null");
        }
    }

    internal class ProcessLogger : IDisposable
    {
        private Stopwatch _stopWatch = new Stopwatch();
        private Logger _logger;
        public ProcessLogger(Logger logger)
        {
            _logger = logger;
            _stopWatch.Start();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
        }

        [Conditional("DEBUG")]
        public void Log(string status)
        {
            _logger.Log($"[{_stopWatch.Elapsed:ss\\.ff}] {status}");
        }
    }
}
