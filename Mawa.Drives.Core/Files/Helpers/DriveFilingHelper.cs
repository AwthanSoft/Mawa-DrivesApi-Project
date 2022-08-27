using Mawa.IO.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mawa.Drives.Files
{
    public static class DriveFilingHelper
    {
        #region Refresh




        #endregion

        #region Drive (Folders & Files) Helper

        //OneFolder 
        public static FolderDriveStructFile[] LoadFolders(string folderPath, bool withFiles)
        {
            var temp_list = new List<FolderDriveStructFile>();
            var RootFolders = DirectoryHelper.GetDirectories(folderPath, SearchOption.TopDirectoryOnly, true);

            foreach (var folder in RootFolders)
            {
                var sub = new FolderDriveStructFile()
                {
                    LocalPath = folder.FullName,
                    IsInLocal = true,
                    Name = folder.Name
                };
                temp_list.Add(sub);
                if (withFiles)
                {
                    sub.Files.AddRange(LoadFiles(folder.FullName));
                }
            }
            return temp_list.ToArray();
        }

        //Recursion
        public static FolderDriveStructFile[] LoadFolders(string folderPath, bool withSubs, bool withFiles)
        {
            var RootFolders = LoadFolders(folderPath, withFiles);
            if (withSubs)
            {
                foreach (var folder in RootFolders)
                {
                    folder.Folders.AddRange(LoadFolders(Path.Combine(folderPath, folder.Name), withFiles, true));
                }
            }
            return RootFolders;
        }



        /**********/
        /// <summary>
        /// search all files in detected folder and return them as FileStruct
        /// </summary>
        /// <param name="rootPath">Folder Path</param>
        /// <returns></returns>
        public static FileDriveStructFile[] LoadFiles(string rootPath)
        {
            var temp_list = new List<FileDriveStructFile>();
            var RootFolders = Directory.GetFiles(rootPath)
                .Select(b => new FileInfo(b)).ToArray();

            foreach (var fileInfo in RootFolders)
            {
                var sub = new FileDriveStructFile()
                {
                    LocalPath = fileInfo.FullName,
                    //DriveFileId = folder.Id,
                    //ParentDriveId = folderId,
                    Name = fileInfo.Name,
                    LocalSize = fileInfo.Length,
                    //MimeType = AppMe.MimeTypes.MimeTypeMap.GetMimeType(fileInfo.Name),
                    IsInLocal = true,

                    //MD5

                };
                temp_list.Add(sub);
            }

            return temp_list.ToArray();
        }
        #endregion

        #region Compare
        public static FolderDriveStructFile CompareExistingAndMerge_ToDriveRoot(FolderDriveStructFile DriveRoot, FolderDriveStructFile LocalRoot)
        {
            FolderDriveStructFile resultt = null;
            //try // it is internal use : as temp
            {
                var drive_fillings_dic = DriveRoot.AllSubStructs_Dic();
                var local_fillings_dic = LocalRoot.AllSubStructs_Dic();
                //Merge
                {
                    //Merge Folders
                    {
                        //CopyFolder
                        var newDriveFolders_list = new List<FolderDriveStructFile>();
                        foreach (var filing in local_fillings_dic.Values.SelectMany(b => b).Where(b => b.driveFileType == DriveFileType.Folder).ToArray())
                        {
                            if (drive_fillings_dic.ContainsKey(filing.StructPath))
                            {
                                foreach (var driveFiling in drive_fillings_dic[filing.StructPath])
                                {
                                    if (driveFiling.driveFileType == DriveFileType.Folder)
                                        driveFiling.IsInLocal = true;
                                }
                            }
                            else
                            {
                                var NewFolder = new FolderDriveStructFile()
                                {
                                    Name = filing.Name,
                                    StructPath = filing.StructPath,
                                    IsInLocal = true,
                                    IsInDrive = false
                                };
                                drive_fillings_dic.Add(NewFolder.StructPath, new List<DriveStructFile>() { NewFolder });
                                newDriveFolders_list.Add(NewFolder);
                            }
                        }

                        //AddNew Folders
                        foreach (var newFolder in newDriveFolders_list)
                        {
                            foreach (var item in drive_fillings_dic[newFolder.Parent_StructPath])
                            {
                                if (item.driveFileType == DriveFileType.Folder)
                                    (item as FolderDriveStructFile).Folders.Add(newFolder);
                            }
                        };
                    }

                    //Merge Files
                    {
                        //CopyFiles
                        foreach (var filing in local_fillings_dic.Values.SelectMany(m => m).Where(b => b.driveFileType == DriveFileType.File).ToArray())
                        {
                            if (drive_fillings_dic.ContainsKey(filing.StructPath))
                            {
                                foreach (var driveFiling in drive_fillings_dic[filing.StructPath])
                                {
                                    if (driveFiling.driveFileType == DriveFileType.File)
                                    {
                                        driveFiling.IsInLocal = filing.IsInLocal;
                                        driveFiling.LocalSize = filing.LocalSize;
                                        //driveFiling.MimeType = filing.MimeType;
                                    }
                                }
                            }
                            else
                            {
                                //FindFolder in local
                                foreach (var driveFolder in drive_fillings_dic[filing.Parent_StructPath])
                                {
                                    if (driveFolder.driveFileType == DriveFileType.Folder)
                                    {
                                        (driveFolder as FolderDriveStructFile).Files.Add(filing as FileDriveStructFile);
                                        drive_fillings_dic.Add(filing.StructPath, new List<DriveStructFile>() { filing });
                                    }
                                    else
                                    {
                                        throw new Exception("file structPath has same Folder!!");
                                    }
                                }
                            }
                        }
                    }
                }
                resultt = DriveRoot;
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return resultt;
        }

        #endregion

        #region Extract
        /// <summary>
        /// Search in all subs and collect FolderStructs and FileStructs
        /// </summary>
        /// <param name="rootFolderStruct">this root where will be start point</param>
        /// <returns>Dictionary result contains all sub core straucts</returns>
        public static Dictionary<string, List<DriveStructFile>> AllSubStructs_Dic(FolderDriveStructFile rootFolderStruct, bool withRoot = true)
        {
            var structs_dic = new Dictionary<string, List<DriveStructFile>>();
            var structs_list = rootFolderStruct.AllSubStructs(withRoot);

            foreach (var strct in structs_list)
            {
                if (!structs_dic.ContainsKey(strct.StructPath))
                {
                    structs_dic.Add(strct.StructPath, new List<DriveStructFile>());
                }
                structs_dic[strct.StructPath].Add(strct);
            }
            return structs_dic;
        }



        #endregion

        #region Load

        public static FolderDriveStructFile LoadStructFiling(string rootPath, bool withSubs, bool withFiles, bool withRefresh)
        {
            var rootFolder = new FolderDriveStructFile()
            {
                LocalPath = rootPath,
                StructPath = string.Empty
            };
            rootFolder.Folders.AddRange(LoadFolders(rootPath, withSubs, withFiles));
            rootFolder.Files.AddRange(LoadFiles(rootPath));

            //Load
            if (withRefresh)
            {
                var structFolders = (new FolderDriveStructFile[] { rootFolder });
                structFolders.Refresh_StructPaths(withFiles);
            }
            return rootFolder;
        }


        #endregion

        #region as temp

        public static FolderDriveStructFile ClearAllFromRoot_WithExcept(
            FolderDriveStructFile rootFolderStruct,
            FolderDriveStructFile ExceptNode,
            Dictionary<string, List<DriveStructFile>> all_cores_dic = null)
        {
            FolderDriveStructFile resultt = null;
            if (all_cores_dic == null)
                all_cores_dic = rootFolderStruct.AllSubStructs_Dic(true);

            //Clear to acheve main Node
            FolderDriveStructFile childNode = ExceptNode;
            FolderDriveStructFile parentNode = null;
            while (true)
            {
                if (string.IsNullOrEmpty(childNode.StructPath))
                {
                    resultt = childNode;
                    break;
                }
                if (all_cores_dic.ContainsKey(childNode.Parent_StructPath))
                {
                    if (all_cores_dic[childNode.Parent_StructPath].Count == 1)
                        parentNode = all_cores_dic[childNode.Parent_StructPath].FirstOrDefault() as FolderDriveStructFile;
                    else
                        throw new Exception("Node has more than one parent");//as temp
                }
                //else
                //{
                //    resultt = childNode;
                //    break;
                //}
                parentNode.Files.Clear();
                parentNode.Folders.Clear();
                parentNode.Folders.Add(childNode);
                childNode = parentNode;
            }
            return resultt;
        }
        #endregion
    }
}
