using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace DataModel.Shared
{
    public class NDLogger : INDLogger
    {
        TelemetryClient _telemetryClient;

        public NDLogger(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
        public void LogEvent(string eventMessage, SeverityLevel severityLevel)
        {
            foreach (KeyValuePair<string, string> keyValuePair in NambaDoctorContext.ContextValues)
            {
                try
                {
                    if (_telemetryClient.Context.GlobalProperties.ContainsKey(keyValuePair.Key))
                    {
                        _telemetryClient.Context.GlobalProperties[keyValuePair.Key] = (keyValuePair.Value);
                    }
                    else
                    {
                        _telemetryClient.Context.GlobalProperties.Add(keyValuePair);
                    }
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackTrace(
                        $"Error:{e}, " +
                        $"trace key:{keyValuePair.Key}{keyValuePair.Value}",
                        SeverityLevel.Error);
                }
            }
            _telemetryClient.TrackTrace(eventMessage, severityLevel);

        }

        public void LogEvent(string eventMessage, string severityLevel = null)
        {
            var _severitiyLevel = SeverityLevel.Information;
            if (severityLevel != null)
            {
                if (severityLevel == "error")
                {
                    _severitiyLevel = SeverityLevel.Error;
                }
            }
            LogEvent(eventMessage, _severitiyLevel);

        }
    }
}

