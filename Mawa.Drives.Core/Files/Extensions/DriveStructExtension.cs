using Mawa.ExceptionsApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mawa.Drives.Files
{
    public static class DriveStructExtension
    {
        #region Refresh

        /// <summary>
        /// Fill StructPathes property in struct with its parent struct.
        /// </summary>
        /// <param name="rootFolderStruct">this root where will be start point</param>
        public static void Refresh_StructPaths(this FolderDriveStructFile rootFolderStruct, bool withFiles)
        {
            foreach (var folder in rootFolderStruct.Folders)
            {
                if (string.IsNullOrEmpty(folder.StructPath))
                {
                    folder.StructPath = Path.Combine(
                        rootFolderStruct.StructPath,
                        folder.Name);
                }
                else
                {

                    if (Path.Combine(
                        rootFolderStruct.StructPath,
                        folder.Name
                        ).Equals(folder.StructPath))
                    {

                    }
                    else
                    {
                        //as temp
                        //think deep why
                        throw new Exception();
                    }
                }
                Refresh_StructPaths(folder, withFiles);
            }

            //Files
            if (!withFiles)
                return;
            foreach (var file in rootFolderStruct.Files)
            {
                if (string.IsNullOrEmpty(file.StructPath))
                {
                    file.StructPath = Path.Combine(
                        rootFolderStruct.StructPath,
                        file.Name
                        //Add Extension
                        );
                }
                else
                {
                    if (Path.Combine(
                        rootFolderStruct.StructPath,
                        file.Name
                        ).Equals(file.StructPath))
                    {

                    }
                    else
                    {
                        //as temp
                        //think deep why
                        throw new Exception();
                    }
                }
            }
        }

        public static void Refresh_StructPaths(this FolderDriveStructFile[] FolderStructs, bool withFiles)
        {
            foreach (var strct in FolderStructs)
            {
                strct.Refresh_StructPaths(withFiles);
            }
        }

        #endregion

        #region Extract

        /// <summary>
        /// Extract all sub FolderStructs as recursion way.
        /// </summary> 
        /// <param name="rootFolderStruct">this root where will be start point</param>
        /// <param name="temp_list">result list appaned</param>
        /// <returns>List result of all sub FolderStructs : it same temp_list if it is not null</returns>
        public static List<FolderDriveStructFile> AllSubFolderStructs(this FolderDriveStructFile rootFolderStruct, List<FolderDriveStructFile> temp_list = null)
        {
            if (temp_list == null)
                temp_list = new List<FolderDriveStructFile>();
            if (rootFolderStruct.Folders.Count > 0)
            {
                temp_list.AddRange(rootFolderStruct.Folders);
                foreach (var sub in rootFolderStruct.Folders)
                {
                    AllSubFolderStructs(sub, temp_list);
                }
            }

            return temp_list;
        }
        public static List<FolderDriveStructFile> AllSubFolderStructs(this FolderDriveStructFile[] rootFolderStructs, bool withRoot = true)
        {
            List<FolderDriveStructFile> temp_list = new List<FolderDriveStructFile>();
            foreach (var item in rootFolderStructs)
            {
                item.AllSubFolderStructs(temp_list);
                if (withRoot)
                    temp_list.Add(item);
            }
            return temp_list;
        }


        //
        public static List<FileDriveStructFile> AllFiles(this FolderDriveStructFile rootFolderStruct)
        {
            var temp_list = new List<FileDriveStructFile>();
            temp_list.AddRange(rootFolderStruct.Files);
            foreach (var folder in rootFolderStruct.AllSubFolderStructs())
            {
                temp_list.AddRange(folder.Files);
            }
            return temp_list;
        }
        public static List<FileDriveStructFile> AllFiles(this FolderDriveStructFile[] rootFolderStructs)
        {
            List<FileDriveStructFile> temp_list = new List<FileDriveStructFile>();
            foreach (var folder in rootFolderStructs)
            {
                temp_list.AddRange(folder.AllFiles());
            }
            return temp_list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFolderStruct"></param>
        /// <param name="withRoot"></param>
        /// <returns></returns>
        public static List<DriveStructFile> AllSubStructs(this FolderDriveStructFile rootFolderStruct, bool withRoot = true)
        {
            var tempList = new List<DriveStructFile>();
            tempList.AddRange(rootFolderStruct.AllSubFolderStructs());
            tempList.AddRange(rootFolderStruct.AllFiles());
            if (withRoot)
                tempList.Add(rootFolderStruct);
            return tempList;
        }


        public static Dictionary<string, List<DriveStructFile>> AllSubStructs_Dic(this FolderDriveStructFile rootFolderStruct, bool withRoot = true)
        {
            return DriveFilingHelper.AllSubStructs_Dic(rootFolderStruct, withRoot);
        }
        #endregion

        #region Search

        public static FolderDriveStructFile[] RemovePathStructFolder(this FolderDriveStructFile[] rootFolderStructs, string StructPath, bool include_LocalPath = true)
        {
            var allFolders = rootFolderStructs.AllSubFolderStructs(true);
            var folders_dic = new Dictionary<string, List<FolderDriveStructFile>>();
            foreach (var folder in allFolders)
            {
                if (!folders_dic.ContainsKey(folder.StructPath))
                {
                    folders_dic.Add(folder.StructPath, new List<FolderDriveStructFile>());
                }
                folders_dic[folder.StructPath].Add(folder);


                if (include_LocalPath)
                {
                    if (!folders_dic.ContainsKey(folder.LocalPath))
                    {
                        folders_dic.Add(folder.LocalPath, new List<FolderDriveStructFile>());
                    }
                    folders_dic[folder.LocalPath].Add(folder);
                }
            }

            //for check
            if (folders_dic.Values.Select(b => b.Count).Max() > 1)
            {
                AsTempException.GeneralException("asTemp : in backup folders_dic has more the value in a Key!!");
            }

            var folder_toDeleted_list = new List<FolderDriveStructFile>();
            var folder_deleted_list = new List<FolderDriveStructFile>();

            if (folders_dic.ContainsKey(StructPath))
            {
                folder_toDeleted_list.AddRange(folders_dic[StructPath]);
            }

            foreach (var nod in folder_toDeleted_list)
            {
                if (folders_dic.ContainsKey(nod.Parent_StructPath))
                {
                    foreach (var parent in folders_dic[nod.Parent_StructPath].ToArray())
                    {
                        if (parent.Folders.Contains(nod))
                        {
                            parent.Folders.Remove(nod);
                            folder_deleted_list.Add(nod);
                        }
                    }
                }
            }

            return folder_deleted_list.ToArray();
        }

        public static FileDriveStructFile[] RemovePathStructFile(this FolderDriveStructFile[] rootFolderStructs, string StructPath, bool include_LocalPath = true)
        {
            var folders_dic = new Dictionary<string, List<FolderDriveStructFile>>();
            {
                var allFolders = rootFolderStructs.AllSubFolderStructs(true);
                foreach (var folder in allFolders)
                {
                    if (!folders_dic.ContainsKey(folder.StructPath))
                    {
                        folders_dic.Add(folder.StructPath, new List<FolderDriveStructFile>());
                    }
                    folders_dic[folder.StructPath].Add(folder);

                    if (include_LocalPath)
                    {
                        if (!folders_dic.ContainsKey(folder.LocalPath))
                        {
                            folders_dic.Add(folder.LocalPath, new List<FolderDriveStructFile>());
                        }
                        folders_dic[folder.LocalPath].Add(folder);
                    }
                }


                //for check
                if (folders_dic.Values.Select(b => b.Count).Max() > 1)
                {
                    AsTempException.GeneralException("asTemp : in backup folders_dic has more the value in a Key!!");
                }
            }

            var files_dic = new Dictionary<string, List<FileDriveStructFile>>();
            {
                var allStructs = rootFolderStructs.AllFiles();

                foreach (var strct in allStructs)
                {
                    if (!files_dic.ContainsKey(strct.StructPath))
                    {
                        files_dic.Add(strct.StructPath, new List<FileDriveStructFile>());
                    }
                    files_dic[strct.StructPath].Add(strct);


                    if (include_LocalPath)
                    {
                        if (!files_dic.ContainsKey(strct.LocalPath))
                        {
                            files_dic.Add(strct.LocalPath, new List<FileDriveStructFile>());
                        }
                        files_dic[strct.LocalPath].Add(strct);
                    }
                }
                //for check
                if (files_dic.Values.Select(b => b.Count).Max() > 1)
                {
                    AsTempException.GeneralException("asTemp : in backup folders_dic has more the value in a Key!!");
                }
            }


            var structs_toDeleted_list = new List<FileDriveStructFile>();
            var structs_deleted_list = new List<FileDriveStructFile>();

            if (files_dic.ContainsKey(StructPath))
            {
                structs_toDeleted_list.AddRange(files_dic[StructPath]);
            }

            foreach (var nod in structs_toDeleted_list)
            {
                if (folders_dic.ContainsKey(nod.Parent_StructPath))
                {
                    foreach (var parent in folders_dic[nod.Parent_StructPath].ToArray())
                    {
                        if (parent.Files.Contains(nod))
                        {
                            parent.Files.Remove(nod);
                            structs_deleted_list.Add(nod);
                        }
                    }
                }
            }

            return structs_deleted_list.ToArray();
        }

        #endregion

    }
}
