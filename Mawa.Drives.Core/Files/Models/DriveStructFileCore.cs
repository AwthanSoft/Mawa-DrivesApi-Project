using System.IO;

namespace Mawa.Drives.Files
{
    //Core
    public interface IDriveStructFile
    {
        //Types
        DriveFileType driveFileType { get; }
        string MimeType { get; }

        //Ids
        string DriveFileId { get; }
        string ParentDriveId { get; }

        //Name
        string Name { get; }

        //Sizes
        //long? DriveSize { get; }
        //long? LocalSize { get; }
        long DriveSize { get; }
        long LocalSize { get; }

        //Existing
        bool IsInDrive { get; }
        bool IsInLocal { get; }

        //Paths
        string StructPath { get; }
        string LocalPath { get; }

        string Parent_StructPath { get; }

    }

    public class DriveStructFile : IDriveStructFile
    {
        #region initial
        public DriveFileType driveFileType { get; set; }

        public DriveStructFile (DriveFileType driveFileType)
        {
            this.driveFileType = driveFileType;
        }
        //public DriveStructFile()
        //{

        //}

        #endregion

        #region BaseInfo

        public string DriveFileId { get; set; }
        public string ParentDriveId { get; set; }
      

        public string MimeType { get; set; }

        public string Name { get; set; }

        //Sizes
        //public virtual long? DriveSize { set; get; }
        //public virtual long? LocalSize { set; get; }
        public virtual long DriveSize { set; get; }
        public virtual long LocalSize { set; get; }

        //Existing
        public bool IsInDrive { set; get; }
        public bool IsInLocal { set; get; }

        //Paths
        public string StructPath { set; get; }
        public string LocalPath { set; get; }

        public string Parent_StructPath
        {
            get
            {
                return Path.GetDirectoryName(StructPath);
            }
        }


        #endregion

    }   

}
