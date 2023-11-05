using System;
using System.IO;
using TWT.Utility;
using UnityEngine;

public static class FileUtility
{
    public static string FILE_NOT_AVAILABLE = "_not_available";
    public static string PREFIX_FILE_INVALID_REASON = " ------ ";

    public static void DeleteAllCacheFile(string path)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        if (!Directory.Exists(path))
            return;

        System.IO.DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
            PlayerPrefsConstant.DeleteVersionContent(dir.Name);
        }
    }

    public static void DeleteAllFile(string path)
    {
        if (!Directory.Exists(path)) return;
        System.IO.DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string[] ignoreFiles = null)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        //get ignore files - invalid files which defined in admin validation
        if (ignoreFiles == null)
        {
            ignoreFiles = new string[0];
            string pathToInvalidFolder = Path.Combine($"{Application.dataPath}/../", "Logs", "invalid_files");
            if (Directory.Exists(pathToInvalidFolder))
            {
                string[] contentInvalidLogs = Directory.GetFiles(pathToInvalidFolder);
                System.Collections.Generic.List<string> ignoreFileList = new System.Collections.Generic.List<string>();
                foreach (var log in contentInvalidLogs)
                {
                    string[] lines = File.ReadAllLines(log);
                    if (lines == null || lines.Length == 0) continue;
                    for (int i = 0; i < lines.Length; ++i)
                        lines[i] = lines[i].Split(new string[] { PREFIX_FILE_INVALID_REASON }, StringSplitOptions.None)[0];
                    ignoreFileList.AddRange(lines);
                }

                ignoreFiles = ignoreFileList.ToArray();

                // foreach(var file in ignoreFileList)
                //     DebugExtension.Log("Invalid:" + file);
                // DebugExtension.Log("===== end log =====");
            }
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (!file.Extension.Equals(".temp") && !file.Extension.Equals(".meta") && Array.FindIndex(ignoreFiles, e => NormalizePath(e) == NormalizePath(file.FullName)) < 0)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    public static bool ValidateFileSize(string file_path, long max_size)
    {
        bool isDontNeedCheckSize = max_size < 0; // ValidateSetting.vr_dont_need_check_size
        if (isDontNeedCheckSize) return true;
        FileInfo file = new FileInfo(file_path);
        return file.Length <= max_size;
    }

    public static void ChangeFileNameToNotAvailable(string file_path)
    {
        // FileInfo file = new FileInfo(file_path);
        // if (!file.Name.Contains(FILE_NOT_AVAILABLE))
        //     file.MoveTo(Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(file.Name) + FILE_NOT_AVAILABLE + file.Extension));
    }

    public static string GetPath(string[] paths)
    {
        if (paths.Length == 0)
        {
            return null;
        }

        var _path = "";
        foreach (var p in paths)
        {
            _path += p + "\n";
        }
        DebugExtension.Log(_path);
        return _path.TrimEnd('\r', '\n');
    }

    public static string OpenSelectFolderDialog(string folder)
    {
#if !UNITY_WEBGL
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", folder, true);
        var folderPath = GetPath(paths);
        if (string.IsNullOrEmpty(folderPath))
            return "";
        var uri = new Uri(folderPath);
        return uri.LocalPath;
#else
        return "";
#endif
    }

    public static void ShowInExplorer(string path)
    {
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    public static string NormalizePath(string path)
    {
        try
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToUpperInvariant();
        }
        catch (Exception e)
        {
            return "";
        }
    }
}