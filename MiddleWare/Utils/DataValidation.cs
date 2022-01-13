using MongoDB.Bson;
using Exceptions = DataModel.Shared.Exceptions;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Utils
{
    public static class DataValidation
    {
        public static void ValidateIncomingId(string Id, IdType type)
        {
            if (type == IdType.Customer)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId custId) == false)
                {
                    throw new ArgumentException("Customer Id was invalid");
                }
            }
            else if (type == IdType.ServiceProvider)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId spId) == false)
                {
                    throw new ArgumentException("Service provider Id was invalid");
                }
            }
            else if (type == IdType.Appointment)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId appId) == false)
                {
                    throw new ArgumentException("Appointment Id was invalid");
                }
            }
            else if (type == IdType.ServiceRequest)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId serviceRequestId) == false)
                {
                    throw new ArgumentException("ServiceRequest Id was invalid");
                }
            }
            else if (type == IdType.Organisation)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId spId) == false)
                {
                    throw new ArgumentException("Organisation Id was invalid");
                }
            }
            else if (type == IdType.Prescription)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId prescId) == false)
                {
                    throw new ArgumentException("Prescription Id was invalid");
                }
            }
            else if (type == IdType.Report)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId reportId) == false)
                {
                    throw new ArgumentException("Report Id was invalid");
                }
            }
            else if (type == IdType.Other)
            {
                if (string.IsNullOrWhiteSpace(Id) || ObjectId.TryParse(Id, out ObjectId spId) == false)
                {
                    throw new ArgumentException($"Id:{Id} was invalid");
                }
            }

        }

        public static void ValidateObject<T>(this T obj)
        {
            var typeOfEntity = obj != null ? obj.GetType() : typeof(T);

            if (obj == null)
            {
                if (typeOfEntity == typeof(Mongo.Appointment))
                {
                    throw new Exceptions.AppointmentDoesNotExistException($"Appointment does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.ServiceProvider))
                {
                    throw new Exceptions.ServiceProviderDoesnotExistsException($"Serviceprovider does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.Customer))
                {
                    throw new Exceptions.CustomerDoesNotExistException($"Customer does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.ServiceProviderProfile))
                {
                    throw new Exceptions.ServiceProviderDoesnotExistsException($"Serviceprovider does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.CustomerProfile))
                {
                    throw new Exceptions.CustomerDoesNotExistException($"Customer does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.ServiceRequest))
                {
                    throw new Exceptions.ServiceRequestDoesNotExistException($"ServiceRequest does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.Organisation))
                {
                    throw new Exceptions.OrganisationDoesNotExistException($"Organisation does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.Report))
                {
                    throw new Exceptions.ReportDoesNotExistException($"Report does not exist");
                }
                else if (typeOfEntity == typeof(Mongo.PrescriptionDocument))
                {
                    throw new Exceptions.PrescriptionDoesNotExistException($"Prescription does not exist");
                }
                else
                {
                    throw new NullReferenceException($"{typeOfEntity} does not exist");
                }
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
