namespace DataModel.Client.Provider.Incoming;

public class TreatmentPlanDocumentIncoming
{
    public AppointmentIncoming Appointment { get; set; }
    public string File { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; } //Pdf, Img
}