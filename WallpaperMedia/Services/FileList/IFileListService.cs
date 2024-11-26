using System.Collections.Generic;
using WallpaperMedia.Models.FileListService;

namespace WallpaperMedia.Services;

public interface IFileListService
{
    List<FileInfo> FileInfoList();
    string GetDownloadsPath();
}