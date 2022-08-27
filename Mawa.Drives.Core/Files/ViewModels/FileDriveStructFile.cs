//using System;
//using AppMe.DriveFiling.Enums;
//namespace AppMe.DriveFiling.ViewModels
//{
//    public class FileDriveStructFile : DriveStructFileCore
//    {
//        #region Initial
//        public FileDriveStructFile():base(DriveFileType.File)
//        {

//        }

//        #endregion

//        //string _MD5;
//        //public string MD5
//        //{
//        //    get => _MD5;
//        //    set
//        //    {
//        //        _MD5 = value;
//        //        NotifyAllPropertiesChanged();
//        //    }
//        //}


//        #region Size

//        public override long? DriveSize
//        {
//            get
//            {
//                if (_DriveSize == null)
//                    return 0;
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
//                    return 0;
//                return _LocalSize;
//            }
//            set
//            {
//                _LocalSize = value;
//                NotifyAllPropertiesChanged();
//            }
//        }

//        #endregion


//        string _Md5Checksum;
//        public string Md5Checksum
//        {
//            get => _Md5Checksum;
//            set
//            {
//                _Md5Checksum = value;
//                NotifyAllPropertiesChanged();
//            }
//        }


//        #region Notify

//        public override void NotifyAllPropertiesChanged()
//        {
//            base.NotifyAllPropertiesChanged();
//            //OnPropertyChanged(nameof(MD5));
//            OnPropertyChanged(nameof(Md5Checksum));
//        }

//        #endregion

//    }
//}
