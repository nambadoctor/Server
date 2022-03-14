namespace DataModel.Client.Provider.Incoming;

public class TreatmentPlanDocumentIncoming
{
    public string TreatmentPlanId { get; set; }
    public string File { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; } //Pdf, Img
}