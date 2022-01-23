using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public class ServiceRequestConverter
    {
        public static Mongo.Report ConvertToMongoReport(ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            var mongoReport = new Mongo.Report();

            mongoReport.ReportId = ObjectId.GenerateNewId();

            var fileInfo = new Mongo.FileInfo();
            {
                fileInfo.FileInfoId = ObjectId.GenerateNewId();
                fileInfo.FileName = reportIncoming.FileName;
                fileInfo.FileType = reportIncoming.FileType;
            };

            mongoReport.FileInfo = fileInfo;

            var detail = new Mongo.ReportDetails();
            detail.Name = reportIncoming.Details;
            detail.Type = reportIncoming.DetailsType;
            mongoReport.Details = detail;

            return mongoReport;
        }

        public static ProviderClientOutgoing.ReportOutgoing ConvertToClientOutgoingReport(Mongo.Report mongoReport, string SasUrl)
        {
            var reportOutgoing = new ProviderClientOutgoing.ReportOutgoing();

            reportOutgoing.ReportId = mongoReport.ReportId.ToString();

            reportOutgoing.Name = mongoReport.FileInfo.FileName;
            reportOutgoing.FileType = mongoReport.FileInfo.FileType;

            if (mongoReport.Details != null)
            {
                reportOutgoing.Details = mongoReport.Details.Details;
                reportOutgoing.DetailsType = mongoReport.Details.Type;
            }

            reportOutgoing.SasUrl = SasUrl;

            return reportOutgoing;
        }

        public static Mongo.PrescriptionDocument ConvertToMongoPrescriptionDocument(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            var mongoPrescriptionDocument = new Mongo.PrescriptionDocument();

            mongoPrescriptionDocument.PrescriptionDocumentId = ObjectId.GenerateNewId();

            var fileInfo = new Mongo.FileInfo();
            {
                fileInfo.FileInfoId = ObjectId.GenerateNewId();
                fileInfo.FileName = prescriptionDocumentIncoming.FileName;
                fileInfo.FileType = prescriptionDocumentIncoming.FileType;
            };

            mongoPrescriptionDocument.FileInfo = fileInfo;

            var detail = new Mongo.PrescriptionDetail();
            detail.Name = prescriptionDocumentIncoming.Details;
            detail.Type = prescriptionDocumentIncoming.DetailsType;
            mongoPrescriptionDocument.PrescriptionDetail = detail;

            return mongoPrescriptionDocument;
        }

        public static ProviderClientOutgoing.PrescriptionDocumentOutgoing ConvertToClientOutgoingPrescriptionDocument(Mongo.PrescriptionDocument mongoPrescriptionDocument, string SasUrl)
        {
            var prescriptionDocumentOutgoing = new ProviderClientOutgoing.PrescriptionDocumentOutgoing();

            prescriptionDocumentOutgoing.PrescriptionDocumentId = mongoPrescriptionDocument.PrescriptionDocumentId.ToString();

            prescriptionDocumentOutgoing.Name = mongoPrescriptionDocument.FileInfo.FileName;
            prescriptionDocumentOutgoing.FileType = mongoPrescriptionDocument.FileInfo.FileType;

            prescriptionDocumentOutgoing.SasUrl = SasUrl;

            return prescriptionDocumentOutgoing;
        }

        public static ProviderClientOutgoing.VitalsOutgoing ConvertToClientOutgoingVitals(Mongo.Vitals mongoVitals, string serviceRequestId)
        {
            var vitals = new ProviderClientOutgoing.VitalsOutgoing();

            vitals.ServiceRequestId = serviceRequestId;

            if (mongoVitals == null)
            {
                return vitals;
            }

            if (mongoVitals.Weight != null)
            {
                var weight = new DataModel.Client.Provider.Common.Weight();
                weight.Type = mongoVitals.Weight.Type;
                weight.Value = mongoVitals.Weight.Value;
                vitals.Weight = weight;
            }
            if (mongoVitals.BloodSugar != null)
            {
                var BloodSugar = new DataModel.Client.Provider.Common.BloodSugar();
                BloodSugar.Type = mongoVitals.BloodSugar.Type;
                BloodSugar.Value = mongoVitals.BloodSugar.Value;
                vitals.BloodSugar = BloodSugar;
            }
            if (mongoVitals.Temperature != null)
            {
                var Temperature = new DataModel.Client.Provider.Common.Temperature();
                Temperature.Type = mongoVitals.Temperature.Type;
                Temperature.Value = mongoVitals.Temperature.Value;
                vitals.Temperature = Temperature;
            }
            if (mongoVitals.BadHabit != null)
            {
                var BadHabit = new DataModel.Client.Provider.Common.Habit();
                BadHabit.AlcoholDetails = mongoVitals.BadHabit.AlcoholDetails;
                BadHabit.SmokingDetails = mongoVitals.BadHabit.SmokingDetails;
                vitals.BadHabit = BadHabit;
            }
            if (mongoVitals.RespiratoryRate != null)
            {
                var RespiratoryRate = new DataModel.Client.Provider.Common.RespiratoryRate();
                RespiratoryRate.Type = mongoVitals.RespiratoryRate.Type;
                RespiratoryRate.Value = mongoVitals.RespiratoryRate.Value;
                vitals.RespiratoryRate = RespiratoryRate;
            }
            if (mongoVitals.BloodPressure != null)
            {
                var BloodPressure = new DataModel.Client.Provider.Common.BloodPressure();
                BloodPressure.Type = mongoVitals.BloodPressure.Type;
                BloodPressure.Value = mongoVitals.BloodPressure.Value;
                vitals.BloodPressure = BloodPressure;
            }
            if (mongoVitals.Saturation != null)
            {
                var Saturation = new DataModel.Client.Provider.Common.Saturation();
                Saturation.Type = mongoVitals.Saturation.Type;
                Saturation.Value = mongoVitals.Saturation.Value;
                vitals.Saturation = Saturation;
            }
            if (mongoVitals.Height != null)
            {
                var Height = new DataModel.Client.Provider.Common.Height();
                Height.Type = mongoVitals.Height.Type;
                Height.Value = mongoVitals.Height.Value;
                vitals.Height = Height;
            }
            if (mongoVitals.Pulse != null)
            {
                var Pulse = new DataModel.Client.Provider.Common.Pulse();
                Pulse.Type = mongoVitals.Pulse.Type;
                Pulse.Value = mongoVitals.Pulse.Value;
                vitals.Pulse = Pulse;
            }

            return vitals;
        }

        public static Mongo.Vitals ConvertToMongoVitals(ProviderClientIncoming.VitalsIncoming vitalsIncoming)
        {
            var vitals = new Mongo.Vitals();

            if (vitalsIncoming == null)
            {
                return vitals;
            }

            if (vitalsIncoming.Weight != null)
            {
                var weight = new Mongo.Weight();
                weight.Type = vitalsIncoming.Weight.Type;
                weight.Value = vitalsIncoming.Weight.Value;
                vitals.Weight = weight;
            }
            if (vitalsIncoming.BloodSugar != null)
            {
                var BloodSugar = new Mongo.BloodSugar();
                BloodSugar.Type = vitalsIncoming.BloodSugar.Type;
                BloodSugar.Value = vitalsIncoming.BloodSugar.Value;
                vitals.BloodSugar = BloodSugar;
            }
            if (vitalsIncoming.Temperature != null)
            {
                var Temperature = new Mongo.Temperature();
                Temperature.Type = vitalsIncoming.Temperature.Type;
                Temperature.Value = vitalsIncoming.Temperature.Value;
                vitals.Temperature = Temperature;
            }
            if (vitalsIncoming.BadHabit != null)
            {
                var BadHabit = new Mongo.Habit();
                BadHabit.AlcoholDetails = vitalsIncoming.BadHabit.AlcoholDetails;
                BadHabit.SmokingDetails = vitalsIncoming.BadHabit.SmokingDetails;
                vitals.BadHabit = BadHabit;
            }
            if (vitalsIncoming.RespiratoryRate != null)
            {
                var RespiratoryRate = new Mongo.RespiratoryRate();
                RespiratoryRate.Type = vitalsIncoming.RespiratoryRate.Type;
                RespiratoryRate.Value = vitalsIncoming.RespiratoryRate.Value;
                vitals.RespiratoryRate = RespiratoryRate;
            }
            if (vitalsIncoming.BloodPressure != null)
            {
                var BloodPressure = new Mongo.BloodPressure();
                BloodPressure.Type = vitalsIncoming.BloodPressure.Type;
                BloodPressure.Value = vitalsIncoming.BloodPressure.Value;
                vitals.BloodPressure = BloodPressure;
            }
            if (vitalsIncoming.Saturation != null)
            {
                var Saturation = new Mongo.Saturation();
                Saturation.Type = vitalsIncoming.Saturation.Type;
                Saturation.Value = vitalsIncoming.Saturation.Value;
                vitals.Saturation = Saturation;
            }
            if (vitalsIncoming.Height != null)
            {
                var Height = new Mongo.Height();
                Height.Type = vitalsIncoming.Height.Type;
                Height.Value = vitalsIncoming.Height.Value;
                vitals.Height = Height;
            }
            if (vitalsIncoming.Pulse != null)
            {
                var Pulse = new Mongo.Pulse();
                Pulse.Type = vitalsIncoming.Pulse.Type;
                Pulse.Value = vitalsIncoming.Pulse.Value;
                vitals.Pulse = Pulse;
            }

            return vitals;
        }

    }
}
