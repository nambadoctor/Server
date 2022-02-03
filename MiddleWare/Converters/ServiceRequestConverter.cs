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
            detail.UploadedDateTime = DateTime.UtcNow;
            mongoReport.Details = detail;

            return mongoReport;
        }

        public static ProviderClientOutgoing.ReportOutgoing ConvertToClientOutgoingReport(Mongo.Report mongoReport, string SasUrl, string ServiceRequestId, string AppointmentId)
        {
            var reportOutgoing = new ProviderClientOutgoing.ReportOutgoing();

            reportOutgoing.ReportId = mongoReport.ReportId.ToString();

            reportOutgoing.FileName = mongoReport.FileInfo.FileName;
            reportOutgoing.FileType = mongoReport.FileInfo.FileType;

            if (mongoReport.Details != null)
            {
                if (mongoReport.Details.UploadedDateTime.HasValue)
                {
                    reportOutgoing.UploadedDateTime = mongoReport.Details.UploadedDateTime.Value;
                }
            }
            reportOutgoing.SasUrl = SasUrl;

            reportOutgoing.ServiceRequestId = ServiceRequestId;
            reportOutgoing.AppointmentId = AppointmentId;

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
            detail.UploadedDateTime = DateTime.UtcNow;
            mongoPrescriptionDocument.PrescriptionDetail = detail;

            return mongoPrescriptionDocument;
        }

        public static ProviderClientOutgoing.PrescriptionDocumentOutgoing ConvertToClientOutgoingPrescriptionDocument(Mongo.PrescriptionDocument mongoPrescriptionDocument, string SasUrl, string ServiceRequestId = "", string AppointmentId = "")
        {
            var prescriptionDocumentOutgoing = new ProviderClientOutgoing.PrescriptionDocumentOutgoing();

            prescriptionDocumentOutgoing.PrescriptionDocumentId = mongoPrescriptionDocument.PrescriptionDocumentId.ToString();

            prescriptionDocumentOutgoing.FileType = mongoPrescriptionDocument.FileInfo.FileType;

            if (mongoPrescriptionDocument.PrescriptionDetail != null)
            {
                if (mongoPrescriptionDocument.PrescriptionDetail.UploadedDateTime.HasValue)
                {
                    prescriptionDocumentOutgoing.UploadedDateTime = mongoPrescriptionDocument.PrescriptionDetail.UploadedDateTime.Value;
                }
            }

            prescriptionDocumentOutgoing.SasUrl = SasUrl;

            prescriptionDocumentOutgoing.AppointmentId = AppointmentId;
            prescriptionDocumentOutgoing.ServiceRequestId = ServiceRequestId;

            return prescriptionDocumentOutgoing;
        }


        public static Mongo.Note ConvertToMongoNote(ProviderClientIncoming.NoteIncoming noteIncoming)
        {
            var mongoNote = new Mongo.Note();

            if (string.IsNullOrEmpty(noteIncoming.NoteId))
            {
                mongoNote.NoteId = ObjectId.GenerateNewId();
                mongoNote.CreatedTime = DateTime.UtcNow;
            }
            else
            {
                mongoNote.NoteId = new ObjectId(noteIncoming.NoteId);
            }

            mongoNote.NoteText = noteIncoming.Note;
            mongoNote.LastModifiedTime = DateTime.UtcNow;
            return mongoNote;
        }

        public static ProviderClientOutgoing.NoteOutgoing ConvertToClientOutgoingNote(Mongo.Note mongoNote, string ServiceRequestId = "", string AppointmentId = "")
        {
            var noteOutgoing = new ProviderClientOutgoing.NoteOutgoing();

            noteOutgoing.NoteId = mongoNote.NoteId.ToString();
            noteOutgoing.Note = mongoNote.NoteText;
            noteOutgoing.CreatedDateTime = mongoNote.CreatedTime;
            noteOutgoing.LastModifiedDateTime = mongoNote.LastModifiedTime;

            noteOutgoing.AppointmentId = AppointmentId;
            noteOutgoing.ServiceRequestId = ServiceRequestId;

            return noteOutgoing;
        }

        public static List<ProviderClientOutgoing.NoteOutgoing> ConvertToClientOutGoingNotes(List<Mongo.Note> mongoNotes, string ServiceRequestId = "", string AppointmentId = "")
        {
            var listOfNotes = new List<ProviderClientOutgoing.NoteOutgoing>();

            if(mongoNotes != null)
            foreach (var note in mongoNotes)
            {
                listOfNotes.Add(ConvertToClientOutgoingNote(note, ServiceRequestId, AppointmentId));
            }

            return listOfNotes;
        }

    }
}
