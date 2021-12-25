using System.Collections.Generic;

namespace DataModel.Shared
{
    public class NambaDoctorContext
    {
        public static Dictionary<string, string> TraceContextValues;

        public static NDUserType ndUserType;

        public static string PhoneNumber;

        public static string FirebaseUserId;

        public static string NDUserId;

        public static bool IsTestUser;

        public static string OrganisationId;

        public static string Designation;

        public INDLogger _NDLogger;

        public NambaDoctorContext(INDLogger NDLogger)
        {
            _NDLogger = NDLogger;
        }
    }
}
