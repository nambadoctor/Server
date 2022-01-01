using System.Collections.Generic;

namespace DataModel.Shared
{
    public class NambaDoctorContext
    {
        public static Dictionary<string, object> TraceContextValues = new Dictionary<string, object>();

        public static string PhoneNumber;

        public static void AddTraceContext(string key, object value)
        {
            TraceContextValues.TryAdd(key, value);
        }

        public NambaDoctorContext()
        {
        }
    }
}
