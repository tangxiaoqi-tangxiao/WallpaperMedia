using System.Collections.Generic;
using WallpaperMedia.Models.FileListService;

namespace WallpaperMedia.Services;

public interface IFileList
{
    List<FileInfo> FileInfoList();
}