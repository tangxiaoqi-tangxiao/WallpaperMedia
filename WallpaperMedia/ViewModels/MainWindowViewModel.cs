using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.Services;
using WallpaperMedia.Services.RePKG;
using WallpaperMedia.ViewModels.MainModels;

namespace WallpaperMedia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileListService _fileListService;
    private readonly IRePKGService _rePKGService;

    public MainWindowViewModel(IFileListService fileListService, IRePKGService rePKGService)
    {
        _fileListService = fileListService;
        _rePKGService = rePKGService;

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
        _FileItems = _fileListService.FileInfoList();
        _DownloadsPath = _fileListService.GetDownloadsPath();
    }

    public void PerformAction()
    {
        List<FileInfo> paths = new();
        foreach (var item in _FileItems)
        {
            if (item.Selected)
                paths.Add(item);
        }

        if (paths.Count > 0)
        {
            foreach (var item in paths)
            {
                if (item.IsProcess)
                {
                    _rePKGService.ExtractFile(item.Path);
                }
                else
                {
                    Console.WriteLine("不需要解包文件路径："+item.Path);
                }
            }
        }
    }
}