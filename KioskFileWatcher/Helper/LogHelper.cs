﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskFileWatcher.Helper
{
    public class LogHelper
    {


        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<LogHelper>
        lazy = new Lazy<LogHelper>(() => new LogHelper()
        {

        });

        public static LogHelper Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void Log(string message, string actionName, string applicationName, string actionGroup)
        {
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, applicationName, message);

            logEvent.Properties["actionName"] = actionName;
            logEvent.Properties["applicationName"] = applicationName;
            logEvent.Properties["actionGroup"] = actionGroup;
            _logger.Factory.ThrowConfigExceptions = true;
            _logger.Factory.ThrowExceptions = true;
            _logger.Log(logEvent);
        }

        public void Warn(string message, string actionName, string applicationName, string actionGroup)
        {
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, applicationName, message);
            logEvent.Properties["actionName"] = actionName;
            logEvent.Properties["applicationName"] = applicationName;
            logEvent.Properties["actionGroup"] = actionGroup;
            _logger.Factory.ThrowConfigExceptions = true;
            _logger.Factory.ThrowExceptions = true;
            _logger.Log(logEvent);
        }

        public void Debug(string message, string actionName, string applicationName, string actionGroup)
        {
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Debug, applicationName, message);
            logEvent.Properties["actionName"] = actionName;
            logEvent.Properties["applicationName"] = applicationName;
            logEvent.Properties["actionGroup"] = actionGroup;
            _logger.Factory.ThrowConfigExceptions = true;
            _logger.Factory.ThrowExceptions = true;
            _logger.Log(logEvent);
        }

        public void Error(Exception message, string actionName, string applicationName, string actionGroup)
        {
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Error, applicationName, message.ToString());
            logEvent.Properties["actionName"] = actionName;
            logEvent.Properties["applicationName"] = applicationName;
            logEvent.Properties["actionGroup"] = actionGroup;
            _logger.Factory.ThrowConfigExceptions = true;
            _logger.Factory.ThrowExceptions = true;
            _logger.Log(logEvent);

        }

    }
}
