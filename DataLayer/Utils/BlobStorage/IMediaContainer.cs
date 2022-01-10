using System.Threading.Tasks;

namespace ND.DataLayer.Utils.BlobStorage
{
    public interface IMediaContainer
    {
        public Task<string> UploadFileToStorage(byte[] fileStream, string fileName);
        public Task<string> DownloadFileFromStorage(string fileName);
    }
}
