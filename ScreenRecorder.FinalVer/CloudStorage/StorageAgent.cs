using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ScreenRecorder.FinalVer.Properties;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace ScreenRecorder.FinalVer.CloudStorage
{
    class StorageAgent : IStorageAgent
    {
        public BlobServiceClient serviseClient { get; private set; }
        BlobContainerClient blobContainer;
        BlobClient client;

        public StorageAgent()
        {
            serviseClient = new BlobServiceClient(Settings.Default.SrorageConnectionString);
            blobContainer = serviseClient.GetBlobContainerClient("localcontainer");
            client = blobContainer.GetBlobClient($@"user-{Settings.Default.UserId}\" + System.IO.Path.GetFileName(Settings.Default.Filename));
        }

        public async void DownloadAsync(string path, string filename)
        {
            BlobClient blob = blobContainer.GetBlobClient(filename);
            var resourse = await blob.DownloadAsync();
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                resourse.Value.Content.CopyTo(fileStream);
            }
        }

        public DataTable LoadFileNames()
        {
            DataTable blobs = new DataTable();

            blobs.Columns.Add("Имя файла");
            foreach (var item in blobContainer.GetBlobs())
            {
                if (item.Name.Contains($@"user-{Settings.Default.UserId}"))
                    blobs.Rows.Add(item.Name.Replace($@"user-{Settings.Default.UserId}/", ""));
            }
            return blobs;
        }

        public async Task UploadAsync(string path)
        {
            await client.UploadAsync(path, new BlobHttpHeaders() { ContentType = path.GetContentType() });
        }

        public void Delete(string filename)
        {
             BlobClient blob = blobContainer.GetBlobClient(filename);
             blob.Delete();
        }
    }
}
