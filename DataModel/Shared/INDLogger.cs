using Microsoft.ApplicationInsights.DataContracts;

namespace DataModel.Shared
{
    public interface INDLogger
    {
        public void LogEvent(string eventMessage, string error = null);
        public void LogEvent(string eventMessage, SeverityLevel severityLevel);

    }
}

