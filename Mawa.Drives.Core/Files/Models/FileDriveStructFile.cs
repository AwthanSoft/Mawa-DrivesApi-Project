namespace Mawa.Drives.Files
{
    public interface IFileDriveStructFile : IDriveStructFile
    {
        string Md5Checksum { get; set; }

    }
    public class FileDriveStructFile : DriveStructFile, IFileDriveStructFile
    {
        public FileDriveStructFile() : base(DriveFileType.File)
        {

        }

        public string Md5Checksum { get; set; }

    }
}
