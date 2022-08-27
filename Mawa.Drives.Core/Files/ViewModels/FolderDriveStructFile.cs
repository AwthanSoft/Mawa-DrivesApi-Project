//using AppMe.DriveFiling.Enums;
//using System.Collections.Generic;
//using System.Linq;

//namespace AppMe.DriveFiling.ViewModels
//{
//    public class FolderDriveStructFile : DriveStructFileCore
//    {
//        #region Initial

//        public List<FolderDriveStructFile> Folders { private set; get; }
//        public List<FileDriveStructFile> Files { private set; get; }

//        public FolderDriveStructFile() : base(DriveFileType.Folder)
//        {
//            Folders = new List<FolderDriveStructFile>();
//            Files = new List<FileDriveStructFile>();
//        }

//        #endregion

//        #region Size
        
//        public override long? DriveSize
//        {
//            get
//            {
//                if(_DriveSize == null)
//                {
//                    return Folders
//                        .Where(b => b.IsInDrive && b.DriveSize != null)
//                        .Select(b => b.DriveSize)
//                        .Sum()
//                    +
//                    Files
//                        .Where(b => b.IsInDrive && b.DriveSize != null)
//                        .Select(b => b.DriveSize)
//                        .Sum();
//                }
//                return _DriveSize;
//            }
//            set
//            {
//                _DriveSize = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        public override long? LocalSize
//        {
//            get
//            {
//                if (_LocalSize == null)
//                {
//                    return Folders
//                        .Where(b => b.IsInLocal && b.LocalSize != null)
//                        .Select(b => b.LocalSize)
//                        .Sum()
//                    +
//                    Files
//                        .Where(b => b.IsInLocal && b.LocalSize != null)
//                        .Select(b => b.LocalSize)
//                        .Sum();
//                }
//                return _LocalSize;
//            }
//            set
//            {
//                _LocalSize = value;
//                NotifyAllPropertiesChanged();
//            }
//        }


//        #endregion

//    }
//}
