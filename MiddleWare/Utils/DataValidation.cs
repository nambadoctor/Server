using MongoDB.Bson;
using Exceptions = DataModel.Shared.Exceptions;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Utils
{
    public static class DataValidation
    {

        public static void ValidateObjectId(string Id, IdType type)
        {
            if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId custId) == false)
            {
                throw new ArgumentException($"{type.ToString()} was invalid");
            }

        }

        public static void ValidateObject<T>(this T obj)
        {
            var typeOfEntity = obj != null ? obj.GetType() : typeof(T);

            if (obj == null)
            {
                throw new Exceptions.ResourceNotFoundException($"{typeOfEntity.FullName} does not exist");
            }
        }

        public static Type GetDeclaredType<T>(
            this T obj)
        {
            return typeof(T);
        }
    }

    public enum IdType
    {
        ServiceProvider,
        Customer,
        Appointment,
        ServiceRequest,
        Organisation,
        Prescription,
        Report,
        Other
    }
}
