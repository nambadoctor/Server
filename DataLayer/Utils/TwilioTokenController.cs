using DataModel.Mongo;
using DataModel.Shared;
using System.Collections.Generic;
using Twilio.Jwt.AccessToken;

namespace DataLayer.Utils
{
    public class TwilioTokenController
    {
        public string roomId;
        public string participantId;
        public string accountSid;
        public string apiKey;
        public string apiSecret;

        public TwilioTokenController(string uid, string appointmentId = "")
        {
            this.participantId = uid;
            this.roomId = appointmentId;
            accountSid = ConnectionConfiguration.TwilioAccountSid;
            apiKey = ConnectionConfiguration.TwilioApiKey;
            apiSecret = ConnectionConfiguration.TwilioApiSecret;
        }
        public string GetVideoRoomToken()
        {
            var identity = participantId;

            // Create a video grant for the token
            var grant = new VideoGrant();
            grant.Room = roomId;
            var grants = new HashSet<IGrant> { grant };

            // Create an Access Token generator
            var token = new Token(accountSid, apiKey, apiSecret, identity: identity, grants: grants);

            return token.ToJwt();
        }
    }
}