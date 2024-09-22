using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppMe.MimeTypes;

using Mawa.Drives.Files;
using Mawa.IO.Helpers;

namespace Mawa.Drives.Disks.Controls
{
    public class PathDeriveControl : AppMe.IDisposableMe.DisposableMeCore, IPathDriveApiService
    {
        #region Initial

        private readonly SemaphoreSlim objectLock = new SemaphoreSlim(1, 1);
        readonly Func<string> RootPath_predicate;
        private readonly string _RootPath;
        public string RootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_RootPath))
                {
                    return RootPath_predicate();
                }
                return _RootPath;
            }
        }

        public PathDeriveControl(string RootPath)
        {
            this._RootPath = RootPath;
        }
        public PathDeriveControl(Func<string> RootPath_predicate)
        {
            this.RootPath_predicate = RootPath_predicate;

            pre_initial();
        }

        private void pre_initial()
        {
            //pre_initial_DriveApiService();

        }

        #endregion

        #region Folders
        //
        public Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId)
        {
            return Task.Run(() => _IsFolderExist_InParent(FolderName, parentsId));
        }
        public Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            return Task.Run(() => _IsFolderExist_InParent(FolderName, parentsId), cancellationToken);
        }
        string _IsFolderExist_InParent(string FolderName, string parentsId)
        {
            var path = Path.Combine(parentsId, FolderName);
            if (!Directory.Exists(path))
                return null;
            return path;
        }

        //
        public Task<string> CreateFolderAsync(string FolderName, string parentsId)
        {
            return Task.Run(() => _CreateFolder(FolderName, parentsId));
        }
        public Task<string> CreateFolderAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            return Task.Run(() => _CreateFolder(FolderName, parentsId), cancellationToken);
        }
        string _CreateFolder(string FolderName, string parentsId)
        {
            var path = Path.Combine(parentsId, FolderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        #endregion

        #region Drive (Folders & Files) Helper

        //OneFolder 
        public Task<FolderDriveStructFile[]> GetFolders_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            return _GetFolders_inFolderAsync(folderId, cancellationToken);
        }
        async Task<FolderDriveStructFile[]> _GetFolders_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            var temp_list = new List<FolderDriveStructFile>();
            var RootFolders = DirectoryHelper.GetDirectories(folderId, SearchOption.TopDirectoryOnly, false);
            foreach (var file in RootFolders)
            {
                var sub = new FolderDriveStructFile()
                {
                    DriveFileId = file.FullName,
                    ParentDriveId = folderId,
                    Name = file.Name,
                    DriveSize = await DirectoryHelper.GetDirectorySizeAsync(file.FullName, false),
                    //MimeType = MimeTypeMap.d,
                    IsInDrive = true

                };
                temp_list.Add(sub);
            }
            return temp_list.ToArray();
        }
        //Recursion
        public Task<FolderDriveStructFile[]> GetFolders_inFolder_WithSubsAsync(string folderId, CancellationToken cancellationToken)
        {
            return _GetFolders_inFolder_WithSubsAsync(folderId, cancellationToken);
        }
        async Task<FolderDriveStructFile[]> _GetFolders_inFolder_WithSubsAsync(string folderId, CancellationToken cancellationToken)
        {
            var RootFolders = await _GetFolders_inFolderAsync(folderId, cancellationToken);
            foreach (var folder in RootFolders)
            {

                folder.Folders.AddRange(await _GetFolders_inFolder_WithSubsAsync(folder.DriveFileId, cancellationToken));
            }
            return RootFolders;
        }


        //GetFiles in Folder
        public Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            return _GetFiles_inFolderAsync(folderId, cancellationToken);
        }
        public Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId)
        {
            return _GetFiles_inFolderAsync(folderId, CancellationToken.None);
        }
        Task<FileDriveStructFile[]> _GetFiles_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            var temp_list = new List<FileDriveStructFile>();
            var dFils = Directory.GetFiles(folderId).Select(b => new FileInfo(b)).ToArray();
            if (dFils.Length > 0)
            {
                temp_list.AddRange(
                    dFils.Select(b =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        return new FileDriveStructFile()
                        {
                            DriveFileId = b.FullName,
                            ParentDriveId = folderId,
                            Name = b.Name,
                            MimeType = MimeTypeMap.TryGetMimeType(b.Extension, out string mimeType) ? mimeType : mimeType,
                            DriveSize = b.Length,
                            Md5Checksum = Hash.ComputeHash.GetHash_MD5(b.FullName),
                            IsInDrive = true
                        };
                    }).ToArray());
            }

            //return temp_list.ToArray();
            return Task.Run(() => temp_list.ToArray(), cancellationToken);
        }


        //
        public Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync()
        {
            return _GetAllExistFilesInDriveAsync(CancellationToken.None);
        }
        public Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync(CancellationToken cancellationToken)
        {
            return _GetAllExistFilesInDriveAsync(cancellationToken);
        }
        Task<FileDriveStructFile[]> _GetAllExistFilesInDriveAsync(CancellationToken cancellationToken)
        {
            var temp_list = new List<FileDriveStructFile>();
            var driveFiles = Directory.GetFiles(this.RootPath).Select(b => new FileInfo(b)).ToArray();
            if (driveFiles == null)
                throw new ArgumentException();

            foreach (var fil in driveFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                temp_list.Add(new FileDriveStructFile
                {
                    DriveFileId = fil.FullName,
                    ParentDriveId = fil.DirectoryName,//check here
                    Name = fil.Name,
                    MimeType = MimeTypeMap.TryGetMimeType(fil.Extension, out string mimeType) ? mimeType : mimeType,
                    DriveSize = fil.Length,
                    Md5Checksum = Hash.ComputeHash.GetHash_MD5(fil.FullName),
                    IsInDrive = true
                });
            }
            //return temp_list.ToArray();
            return Task.Run(() => temp_list.ToArray(), cancellationToken);
        }


        //
        public Task<FolderDriveStructFile> LoadFolder_asStructAsync(string folderId, bool withSubFolder, bool withFiles, CancellationToken cancellationToken)
        {
            return _LoadFolder_asStructAsync(folderId, withSubFolder, withFiles, cancellationToken);
        }
        async Task<FolderDriveStructFile> _LoadFolder_asStructAsync(string folderId, bool withSubFolder, bool withFiles, CancellationToken cancellationToken)
        {
            //var driveFile = await _apiCtrl.GetFolder(folderId, cancellationToken);

            var driveFile = new DirectoryInfo(folderId);
            if (!driveFile.Exists)
                throw new DriveDiskException($"Folder({folderId}) not exist in drive.")
                {
                    ExceptionType = "FolderExisting"
                };

            var rootStruct = new FolderDriveStructFile()
            {
                DriveFileId = driveFile.FullName,
                ParentDriveId = (driveFile.Parent != null) ? driveFile.Parent.FullName : null,
                Name = driveFile.Name,
                DriveSize = await DirectoryHelper.GetDirectorySizeAsync(driveFile.FullName, false),
                //MimeType = driveFile.MimeType,
                IsInDrive = true
            };

            if (withSubFolder)
            {
                var folders = await _GetFolders_inFolderAsync(folderId, cancellationToken);
                rootStruct.Folders.AddRange(folders);
                foreach (var fldr in folders)
                {
                    var tempFldrs = await _GetFolders_inFolder_WithSubsAsync(fldr.DriveFileId, cancellationToken);
                    fldr.Folders.AddRange(tempFldrs);
                };
            }

            if (withFiles)
            {
                var folders = new FolderDriveStructFile[] { rootStruct }.AllSubFolderStructs(true);
                foreach (var fldr in folders)
                {
                    var files = await _GetFiles_inFolderAsync(fldr.DriveFileId, cancellationToken);
                    fldr.Files.AddRange(files);
                }
            }

            return rootStruct;
        }

        #endregion

        #region Files

        public Task<FileDriveStructFile> UploadFileToDriveAsync(string srcFilePath, string mimeType, string ParentId, CancellationToken cancellationToken)
        {
            return _UploadFileToDriveAsync(srcFilePath, mimeType, ParentId, cancellationToken);
        }
        async Task<FileDriveStructFile> _UploadFileToDriveAsync(string srcFilePath, string mimeType, string ParentId, CancellationToken cancellationToken)
        {
#if DEBUG
            //FileHelper.FileCopy(srcFilePath, ParentId, true, false, false);
#endif
            var destFileName = Path.Combine(ParentId, Path.GetFileName(srcFilePath));
            FileHelper.FileCopy(srcFilePath, destFileName, true, false, true);
            await Task.Delay(100, cancellationToken);
            var fileInfo = new FileInfo(destFileName);
            return new FileDriveStructFile()
            {
                DriveFileId = fileInfo.FullName,
                DriveSize = fileInfo.Length,
                IsInDrive = fileInfo.Exists,
                Md5Checksum = Hash.ComputeHash.GetHash_MD5(fileInfo.FullName),
                Name = fileInfo.Name,
                ParentDriveId = fileInfo.DirectoryName,//check here
                MimeType = MimeTypeMap.TryGetMimeType(fileInfo.Extension, out string mim) ? mim : null
            };
        }

        public Task<FileDriveStructFile> CopyFileAsync(string originFileId, string copyTitle, string toFolderId, CancellationToken cancellationToken)
        {
            return _CopyFileAsync(originFileId, copyTitle, toFolderId, cancellationToken);
        }
        async Task<FileDriveStructFile> _CopyFileAsync(string originFileId, string copyTitle, string toFolderId, CancellationToken cancellationToken)
        {
            var destFileName = Path.Combine(toFolderId, copyTitle);
            FileHelper.FileCopy(originFileId, destFileName, true, false, true);
            await Task.Delay(100, cancellationToken);
            var fileInfo = new FileInfo(destFileName);
            return new FileDriveStructFile()
            {
                DriveFileId = fileInfo.FullName,
                DriveSize = fileInfo.Length,
                IsInDrive = fileInfo.Exists,
                Md5Checksum = Hash.ComputeHash.GetHash_MD5(fileInfo.FullName),
                Name = fileInfo.Name,
                ParentDriveId = fileInfo.DirectoryName,//check here
                MimeType = MimeTypeMap.TryGetMimeType(fileInfo.Extension, out string mim) ? mim : null
            };
        }
        #endregion


        #region Access

        public async Task<bool> AccessServiceAsync()
        {
            return (await this.GetDriveAboutAsync(CancellationToken.None)).IsReady;
        }

        public async Task<bool> AccessServiceAsync(CancellationToken cancellationToken)
        {
            return (await this.GetDriveAboutAsync(cancellationToken)).IsReady;
        }
        #endregion

        #region About
        public async Task<PathDriveAbout> GetDriveAboutAsync(CancellationToken cancellationToken)
        {
            var path = this.RootPath;
            PathDriveAbout result = null;
            if (!string.IsNullOrEmpty(path))
            {
                //Check is Hard
                if (Directory.Exists(path))
                {
                    result = new PathDriveAbout()
                    {
                        TargetPath = path,
                        IsReady = false,
                        DriveType = DriveType.Unknown,
                    };
                    result.IsReady = true;

                    //Drive
                    var root = Directory.GetDirectoryRoot(path);
                    var driveInfo = DriveInfo.GetDrives().Where(b =>
                        b.Name.Equals(root)
                        //|| string.Format("({0}) {1}", b.Name, b.VolumeLabel).Equals(root)
                        || b.RootDirectory.FullName.Equals(root))
                        .FirstOrDefault();
                    if (driveInfo != null)
                    {
                        result.VolumeLabel = driveInfo.VolumeLabel;
                        result.Name = driveInfo.Name;
                        result.DriveFormat = driveInfo.DriveFormat;
                        result.DriveType = driveInfo.DriveType;
                        result.IsReady = driveInfo.IsReady;
                        result.RootDirectory = (driveInfo.RootDirectory != null) ? driveInfo.RootDirectory.FullName : null;

                        result.AvailableFreeSpace = driveInfo.AvailableFreeSpace;
                        result.TotalFreeSpace = driveInfo.TotalFreeSpace;
                        result.TotalSize = driveInfo.TotalSize;
                    }

                    //
                    if (driveInfo.Name == path)
                    {
                        result.isRootPath = true;
                    }

                    //size
                    {
                        if (result.isRootPath && driveInfo != null)
                        {
                            result.PathUsedSize = (driveInfo.TotalSize - driveInfo.AvailableFreeSpace);
                        }
                        else
                        {
                            result.PathUsedSize = await DirectoryHelper.GetDirectorySizeAsync(path, false);
                        }
                    }
                }
            }
            else
            {

            }
            //await Task.Delay(200);
            return result;
        }

        #endregion

        #region Dispose

        protected override void Dispose_OnFreeOtherManaged()
        {
            base.Dispose_OnFreeOtherManaged();
        }

        protected override void Dispose_OnFreeUnManaged()
        {
            base.Dispose_OnFreeUnManaged();
            this.objectLock.Dispose();
        }
        ~PathDeriveControl()
        {
            Dispose(false);
        }

        #endregion
    }
}
