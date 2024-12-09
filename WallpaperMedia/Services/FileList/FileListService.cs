using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WallpaperMedia.Configs;
using WallpaperMedia.CustomException;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.Utils;

namespace WallpaperMedia.Services.FileListService;

public class FileListService : IFileListService
{
    private readonly RegeditHelp _regeditHelp = new(FileConfig.SteamRegedit);

    public List<FileInfoModel> FileInfoList()
    {
        if (string.IsNullOrWhiteSpace(GlobalConfig.config.SteamPath))
        {
            string folderPath = _regeditHelp.Read("", "SteamPath0")?.ToString().Replace('/', '\\');
            if (folderPath == null)
            {
                throw new WallpaperPathError("未找到stema", WallpaperPathErrorEnum.Steam);
            }
            GlobalConfig.config.SteamPath = folderPath;
        }

        string wallpaperPath = Path.Combine(GlobalConfig.config.SteamPath, FileConfig.WallpaperPath);

        return ParseFile(wallpaperPath);
    }

    //获取系统下载文件夹
    public string GetDownloadsPath() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    //私有方法
    //解析Wallpaper壁纸文件
    private List<FileInfoModel> ParseFile(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new WallpaperPathError("未找到Wallpaper Engine下载壁纸", WallpaperPathErrorEnum.Wallpapers);

        List<FileInfoModel> fileInfos = new List<FileInfoModel>();

        // 获取所有子目录
        string[] directories = Directory.GetDirectories(folderPath, "*.*", SearchOption.TopDirectoryOnly);

        const string config = "project.json";
        const string typeStr = "scene";
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
                FileInfoJsonModel? fileInfoJson =
                    JsonSerializer.Deserialize(jsonStr, FileInfoJsonModelContext.Default.FileInfoJsonModel);
                bool isScene = fileInfoJson?.type.ToLower() == typeStr;
                string path = Path.Combine(directory, isScene ? (typeStr + ".pkg") : fileInfoJson?.file ?? "");
                if (!File.Exists(path))
                    continue;
                fileInfos.Add(new FileInfoModel
                {
                    Title = fileInfoJson?.title ?? "",
                    Path = path,
                    ThumbnailPath = Path.Combine(directory, fileInfoJson?.preview ?? ""),
                    IsProcess = fileInfoJson?.type.ToLower() == typeStr,
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