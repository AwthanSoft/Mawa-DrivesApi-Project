using Mawa.Drives.Files;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mawa.Drives.Disks
{
    public interface IPathDriveApiService : Mawa.Drives.APIs.IDriveApiService<PathDriveAbout>, IDisposable
    {
        string RootPath { get; }
        Task<FileDriveStructFile> UploadFileToDriveAsync(string srcFilePath, string mimeType, string ParentId, CancellationToken cancellationToken);
        Task<FileDriveStructFile> CopyFileAsync(string originFileId, string copyTitle, string toFolderId, CancellationToken cancellationToken);
        //Task<PathDriveAbout> GetDriveAboutAsync(CancellationToken cancellationToken);
    }
}
