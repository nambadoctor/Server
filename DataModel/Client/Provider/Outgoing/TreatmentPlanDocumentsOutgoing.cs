namespace DataModel.Client.Provider.Outgoing;

public class TreatmentPlanDocumentsOutgoing
{
    public string TreatmentPlanId { get; set; }
    
    public string TreatmentPlanDocumentId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; } //Pdf, Img
    public string SasUrl { get; set; }
    
    public string ServiceRequestId { get; set; }
}