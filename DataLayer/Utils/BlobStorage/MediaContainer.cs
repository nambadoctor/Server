using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using DataModel.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ND.DataLayer.Utils.BlobStorage
{
    public class MediaContainer : IMediaContainer
    {
        private BlobServiceClient blobServiceClient;

        private BlobContainerClient containerClient;

        private ILogger logger;


        public MediaContainer(ILogger<MediaContainer> logger)
        {
            blobServiceClient = new BlobServiceClient(ConnectionConfiguration.BlobStorageConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(ConnectionConfiguration.BlobContainerName);

            this.logger = logger;
        }

        public async Task<string> UploadFileToStorage(byte[] fileStream, string fileName)
        {
            dynamic result;
            using (var stream = new MemoryStream(fileStream, writable: false))
            {
                result = await containerClient.UploadBlobAsync(fileName, stream);
            }

            return result.ToString();
        }

        public async Task<string> DownloadFileFromStorage(string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            var uri = await GetServiceSasUriForBlob(blobClient);
            return uri.AbsoluteUri;
        }

        private Task<Uri> GetServiceSasUriForBlob(BlobClient blobClient,
            string storedPolicyName = null)
        {
            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Console.WriteLine();

                return Task.FromResult(sasUri);
            }
            else
            {
                return null;
            }
        }

        private async Task<byte[]> DownloadFileAsBytesFromStorage(string fileName)
        {
            try
            {
                BlobClient blob = containerClient.GetBlobClient(fileName);
                BlobDownloadInfo blobDownload = await blob.DownloadAsync();
                var content = blobDownload.Content;

                byte[] result;
                using (var memoryStream = new MemoryStream())
                {
                    content.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
