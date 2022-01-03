using MongoDB.Bson;
using Server = DataModel.Mongo;

namespace MiddleWare.Utils
{
    public class ProcessDBCollection
    {
        public static List<string> GetMemberIds(Server.Organisation organisation)
        {
            var listOfServiceProviderIds = new List<string>();

            if (organisation.Members != null)
            {
                foreach (var member in organisation.Members)
                {
                    listOfServiceProviderIds.Add(member.ServiceProviderId);
                }
            }

            if (listOfServiceProviderIds.Count == 0)
            {
                throw new InvalidDataException($"No members for this organisation id:{organisation.OrganisationId}");
            }

            return listOfServiceProviderIds;
        }

        public static List<ObjectId> ConvertStringListToObjectIdList(List<string> listOfStringIds)
        {
            var objectIdList = new List<ObjectId>();
            if (listOfStringIds != null)
            { 
                foreach (var objectId in listOfStringIds)
                {
                    objectIdList.Add(new ObjectId(objectId));
                }
            }
            return objectIdList;
        }
    }




}
