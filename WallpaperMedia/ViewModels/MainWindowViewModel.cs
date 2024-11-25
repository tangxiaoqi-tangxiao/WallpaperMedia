using System.Collections.Generic;
using System.Collections.ObjectModel;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.Services;
using WallpaperMedia.ViewModels.MainModels;

namespace WallpaperMedia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileList _fileList;

    public MainWindowViewModel(IFileList fileList)
    {
        _fileList = fileList;

        //初始化
        Initialize();
    }

    public string _DownloadsPath { get; set; }

    /// <summary>
    /// 获取<see cref="ToDoItem"/>的集合，该集合允许添加和删除项目
    /// </summary>
    public List<FileInfo> _FileItems { get; set; } = new();

    public void Initialize()
    {
        _FileItems = _fileList.FileInfoList();
        _DownloadsPath = _fileList.GetDownloadsPath();
    }
}