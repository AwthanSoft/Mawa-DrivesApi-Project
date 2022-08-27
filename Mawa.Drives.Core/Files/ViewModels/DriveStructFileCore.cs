//using System;
//using System.IO;
//using AppMe.DriveFiling.Enums;

//namespace Mawa.Drives.Files.ViewModels
//{
//    public abstract class DriveStructFileCore : AppMe.ComponentModel.NotifyPropertyChangedMeCore
//    {
//        #region initial
//        //BackupType
//        readonly DriveFileType _driveFileType;
//        public DriveFileType driveFileType => _driveFileType;

//        public DriveStructFileCore(DriveFileType driveFileType)
//        {
//            this._driveFileType = driveFileType;
//        }

//        #endregion

//        #region BaseInfo
        
//        string _DriveFileId;
//        public string DriveFileId
//        {
//            get => _DriveFileId;
//            set
//            {
//                _DriveFileId = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        string _ParentDriveId;
//        public string ParentDriveId
//        {
//            get => _ParentDriveId;
//            set
//            {
//                _ParentDriveId = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        string _MimeType;
//        public string MimeType
//        {
//            get => _MimeType;
//            set
//            {
//                _MimeType = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        string _Name;
//        public string Name
//        {
//            get => _Name;
//            set
//            {
//                _Name= value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        //Sizes
//        protected long? _DriveSize;
//        public abstract long? DriveSize { set; get; }
//        protected long? _LocalSize;
//        public abstract long? LocalSize { set; get; }


//        bool _IsInDrive = false;
//        public bool IsInDrive
//        {
//            get => _IsInDrive;
//            set
//            {
//                _IsInDrive = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        bool _IsInLocal = false;
//        public bool IsInLocal
//        {
//            get => _IsInLocal;
//            set
//            {
//                _IsInLocal = value;
//                NotifyAllPropertiesChanged();
//            }
//        }



//        //StructPath
//        string _StructPath;
//        public string StructPath
//        {
//            get => _StructPath;
//            set
//            {
//                _StructPath = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        public string LocalPath
//        {
//            get
//            {
//                return Path.Combine(
//                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
//                    StructPath
//                    );
//            }
//        }

//        public string Parent_StructPath
//        {
//            get
//            {
//                return Path.GetDirectoryName(StructPath);
//            }
//        }


//        #endregion


//        #region INotify fire

//        public override void NotifyAllPropertiesChanged()
//        {
//            OnPropertyChanged(nameof(DriveFileId));
//            OnPropertyChanged(nameof(ParentDriveId));
//            OnPropertyChanged(nameof(MimeType));
//            OnPropertyChanged(nameof(Name));
            
//            OnPropertyChanged(nameof(DriveSize));
//            OnPropertyChanged(nameof(LocalSize));

//            OnPropertyChanged(nameof(StructPath));
//            OnPropertyChanged(nameof(LocalPath));
//        }

//        #endregion
//    }   
//}
