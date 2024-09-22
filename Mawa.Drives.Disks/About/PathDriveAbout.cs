namespace Mawa.Drives.Disks
{
    public interface IPathDriveAbout : About.IDriveAboutCore
    {
        string VolumeLabel { get; }
        string Name { get; }
        System.IO.DriveType DriveType { get; }
        string RootDirectory { get; }
        string DriveFormat { get; }


        long AvailableFreeSpace { get; }
        long TotalFreeSpace { get; }
        long TotalSize { get; }

        //Path
        long PathUsedSize { get; }
        string TargetPath { get; }

        bool IsReady { get; }
        /// <summary>
        /// if it path of hard disk as root path like <b>("C:\")</b>
        /// </summary>
        bool isRootPath { get; }
    }

    public class PathDriveAbout : IPathDriveAbout
    {
        public string VolumeLabel { get; set; }
        public string Name { get; set; }
        public System.IO.DriveType DriveType { get; set; }
        public string RootDirectory { get; set; }
        public string DriveFormat { get; set; }

        //Drive
        public long AvailableFreeSpace { get; set; }
        public long TotalFreeSpace { get; set; }
        public long TotalSize { get; set; }
        //Path
        public long PathUsedSize { get; set; }
        public string TargetPath { get; set; }

        public bool IsReady { set; get; }

        public bool isRootPath { get; set; }
    }
}
