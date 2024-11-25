using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WallpaperMedia.Configs;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.Utils;
using FileInfo = WallpaperMedia.Models.FileListService.FileInfo;

namespace WallpaperMedia.Services.FileListService;

public class FileList(RegeditHelp regeditHelp) : IFileList
{
    public List<FileInfo> FileInfoList()
    {
        string folderPath = regeditHelp.Read("", "SteamPath")?.ToString().Replace('/', '\\');
        if (folderPath == null)
        {
            throw new Exception("未找到stema");
        }

        folderPath = Path.Combine(folderPath, GlobalConfig.WallpaperPath);

        return ParseFile(folderPath);
    }

    //获取系统下载文件夹
    public string GetDownloadsPath() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    //解析Wallpaper壁纸文件
    private List<FileInfo> ParseFile(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new Exception("未找到Wallpaper Engine下载壁纸");

        List<FileInfo> fileInfos = new List<FileInfo>();

        // 获取所有子目录
        string[] directories = Directory.GetDirectories(folderPath, "*.*", SearchOption.TopDirectoryOnly);

        const string config = "project.json";
        foreach (string directory in directories)
        {
            //判断config文件是否存在
            string configPath = Path.Combine(directory, config);
            // 检查文件是否存在
            if (!File.Exists(configPath))
                continue;
            string jsonStr = File.ReadAllText(configPath);
            if (string.IsNullOrWhiteSpace(jsonStr))
                continue;
            try
            {
                FileInfoJson? fileInfoJson =
                    JsonSerializer.Deserialize(jsonStr, FileInfoJsonContext.Default.FileInfoJson);
                bool isScene = fileInfoJson?.type.ToLower() == "scene";
                string path = Path.Combine(directory, isScene ? "scene.pkg" : fileInfoJson?.file ?? "");
                if (!File.Exists(path))
                    continue;
                fileInfos.Add(new FileInfo
                {
                    Title = fileInfoJson?.title ?? "",
                    Path = path,
                    ThumbnailPath = Path.Combine(directory, fileInfoJson?.preview ?? ""),
                    IsProcess = fileInfoJson?.type == "scene",
                    Type = fileInfoJson?.type ?? ""
                });
            }
            catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException or JsonException)
            {
                Console.WriteLine("json序列化错误：" + e.Message);
                throw;
            }
        }

        return fileInfos;
    }
}