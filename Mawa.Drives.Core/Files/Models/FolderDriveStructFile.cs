using System.Collections.Generic;
using System.Linq;

namespace Mawa.Drives.Files
{
    public interface IFolderDriveStructFile : IDriveStructFile
    {
        IReadOnlyList<IFolderDriveStructFile> Folders { get; }
        IReadOnlyList<IFileDriveStructFile> Files { get; }
    }

    public class FolderDriveStructFile : DriveStructFile, IFolderDriveStructFile
    {
        #region Initial

        private readonly List<FolderDriveStructFile> _Folders = new List<FolderDriveStructFile>();
        private readonly List<FileDriveStructFile> _Files = new List<FileDriveStructFile>();

        public List<FolderDriveStructFile> Folders => _Folders;
        public List<FileDriveStructFile> Files => _Files;

        IReadOnlyList<IFolderDriveStructFile> IFolderDriveStructFile.Folders => _Folders;
        IReadOnlyList<IFileDriveStructFile> IFolderDriveStructFile.Files => _Files;

        public FolderDriveStructFile() : base(DriveFileType.Folder)
        {
            this._Folders = new List<FolderDriveStructFile>();
            this._Files = new List<FileDriveStructFile>();
        }

        #endregion

        #region Size

        public override long DriveSize
        {
            get
            {
                return Folders
                      .Where(b => b.IsInDrive)
                      .Select(b => b.DriveSize)
                      .Sum()
                  +
                  Files
                      .Where(b => b.IsInDrive)
                      .Select(b => b.DriveSize)
                      .Sum();
            }
        }

        public override long LocalSize
        {
            get
            {
                return Folders
                      .Where(b => b.IsInLocal)
                      .Select(b => b.LocalSize)
                      .Sum()
                  +
                  Files
                      .Where(b => b.IsInLocal)
                      .Select(b => b.LocalSize)
                      .Sum();
            }
        }

        #endregion

    }
}
