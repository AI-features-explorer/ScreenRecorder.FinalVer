using Azure.Storage.Blobs;
using System.Data;
using System.Threading.Tasks;

namespace ScreenRecorder.FinalVer.CloudStorage
{
    interface IStorageAgent
    {
        BlobServiceClient serviseClient { get; }
        Task UploadAsync(string path);
        void DownloadAsync(string path, string filename);
        DataTable LoadFileNames();
    }
}
