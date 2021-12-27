using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NambaDoctorServiceTests.Services.Auth
{
    public class AuthInfo
    {
        public string idToken { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
        public bool isNewUser { get; set; }
        public string phoneNumber { get; set; }
    }
}
