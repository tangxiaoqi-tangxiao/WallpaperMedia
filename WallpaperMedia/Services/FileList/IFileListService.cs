using System.Collections.Generic;
using WallpaperMedia.Models.FileListService;

namespace WallpaperMedia.Services;

public interface IFileListService
{
    List<FileInfoModel> FileInfoList();
    string GetDownloadsPath();
}