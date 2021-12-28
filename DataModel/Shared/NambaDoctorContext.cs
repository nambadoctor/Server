using System.Collections.Generic;

namespace DataModel.Shared
{
    public class NambaDoctorContext
    {
        public static Dictionary<string, string> TraceContextValues;

        public static string PhoneNumber;

        public INDLogger _NDLogger;

        public NambaDoctorContext(INDLogger NDLogger)
        {
            _NDLogger = NDLogger;
        }
    }
}
