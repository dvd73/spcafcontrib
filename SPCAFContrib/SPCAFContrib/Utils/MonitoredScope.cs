using System.Globalization;
using SPCAF.Sdk.Logging;
using System;
using System.Diagnostics;

namespace SPCAFContrib.Utils
{
    public class MonitoredScope : IDisposable
    {
        #region contructors

        public MonitoredScope(string scopeName)
        {
            _scopeName = scopeName;
            _stopwatch = Stopwatch.StartNew();

            LoggingService.Log(LogLevel.Info,
                               string.Format("MonitoredScope: [{0}] started. TimeStamp:[{1}]",
                                new object[] { _scopeName, DateTime.Now }));
        }

        #endregion

        #region properties

        private readonly string _scopeName;
        private readonly Stopwatch _stopwatch;
        private long? _lastCheckElapsedMilliseconds;

        #endregion

        #region methods

        public void MakeCheck(string ckeckDescription)
        {
            MakeCheck(ckeckDescription, -1);
        }

        public void MakeCheck(string ckeckDescription, int warnCheckInMilliseconds)
        {
            long currentCheckTime = _stopwatch.ElapsedMilliseconds;
            long lastCheckExcecutionTime = _lastCheckElapsedMilliseconds.HasValue ? currentCheckTime - _lastCheckElapsedMilliseconds.Value : 0;

            string checkMessage = string.Format("MonitoredScope: [{0}] finished. Check:[{1}] Excecution time:[{2} ms] From last check:[{3}]",
                    new object[]
                    {
                        _scopeName,
                        ckeckDescription,
                        currentCheckTime.ToString(CultureInfo.InvariantCulture),
                        lastCheckExcecutionTime
                    });

            if (warnCheckInMilliseconds > 0 && lastCheckExcecutionTime >= warnCheckInMilliseconds)
                LoggingService.Log(LogLevel.Warn, checkMessage);
            else
                LoggingService.Log(LogLevel.Info, checkMessage);

            _lastCheckElapsedMilliseconds = currentCheckTime;
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            LoggingService.Log(LogLevel.Info,
                               string.Format("MonitoredScope: [{0}] finished. TimeStamp:[{1}]. Excecution time:[{2} ms]",
                                new object[] { _scopeName, DateTime.Now, _stopwatch.ElapsedMilliseconds }));
        }

        #endregion
    }
}
