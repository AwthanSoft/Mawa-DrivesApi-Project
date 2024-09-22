using System.Threading;
using System.Threading.Tasks;

using Mawa.Drives.Files;

namespace Mawa.Drives.APIs
{
    public interface IDriveApiService
    {
        #region Folders

        /// <summary>
        /// to check is folder ixist in specify folder.
        /// </summary>
        /// <param name="FolderName">Folder Name that need to check</param>
        /// <param name="parentsId">Parent Folder Id that will searched in</param>
        /// <returns>Return Folder Id if exist or null</returns>
        Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId);
        Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId, CancellationToken cancellationToken);
        //Task<bool> IsFolderIdExistAsync(string FolderId, CancellationToken cancellationToken);

        /// <summary>
        /// Create Folder in specify directory.
        /// </summary>
        /// <param name="FolderName">new folder name</param>
        /// <param name="parentsId">parent folder id</param>
        /// <returns>new folder id</returns>
        Task<string> CreateFolderAsync(string FolderName, string parentsId);
        Task<string> CreateFolderAsync(string FolderName, string parentsId, CancellationToken cancellationToken);

        //Task<DriveFile[]> GetFolders_InParentAsync(string parentsId, CancellationToken cancellationToken);
        //Task<DriveFile[]> GetFolders_InParentAsync(string parentsId);

        #endregion

        #region Files

        //
        //Task<DriveFile[]> GetFiles_InParentAsync(string parentsId, CancellationToken cancellationToken);
        //Task<DriveFile[]> GetFiles_InParentAsync(string parentsId);


        ///// <summary>
        ///// Copy an existing file.
        ///// </summary>
        ///// <param name="service">Drive API service instance.</param>
        ///// <param name="originFileId">ID of the origin file to copy.</param>
        ///// <param name="copyTitle">Title of the copy.</param>
        ///// <returns>The copied file, null is returned if an API error occurred</returns>
        //Task<DriveFile> CopyFileAsync(String originFileId, String copyTitle, string toFolderId, CancellationToken cancellationToken);


        //Task<DriveFile> UploadLargeFileToDriveAsync(string filePact, string mimeType, string parent,
        //    Func<bool> IsResumeng_Predicate,
        //    Action<Google.Apis.Upload.IUploadProgress> ProgressChanged_Action,
        //    CancellationToken cancellationToken);


        #endregion

        #region Drive (Folders & Files) Helper

        //
        Task<FolderDriveStructFile[]> GetFolders_inFolderAsync(string folderId, CancellationToken cancellationToken);
        //Recursion
        Task<FolderDriveStructFile[]> GetFolders_inFolder_WithSubsAsync(string folderId, CancellationToken cancellationToken);


        //GetFiles in Folder
        Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId, CancellationToken cancellationToken);
        //Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId);


        //
        Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync(CancellationToken cancellationToken);
        //Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync();


        //
        Task<FolderDriveStructFile> LoadFolder_asStructAsync(string folderId, bool withSubFolder, bool withFiles, CancellationToken cancellationToken);

        #endregion

        #region About Drive

        Task<bool> AccessServiceAsync();
        Task<bool> AccessServiceAsync(CancellationToken cancellationToken);

        #endregion
    }

    public interface IDriveApiService<TDriveAbout> : IDriveApiService
        where TDriveAbout : class, About.IDriveAboutCore
    {

        Task<TDriveAbout> GetDriveAboutAsync(CancellationToken cancellationToken);
    }
}
