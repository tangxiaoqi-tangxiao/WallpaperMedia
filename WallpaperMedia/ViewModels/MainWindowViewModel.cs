using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WallpaperMedia.Configs;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.Services;
using WallpaperMedia.Services.RePKG;
using WallpaperMedia.Utils;

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
    public List<FileInfoModel> _FileItems { get; set; } = new();

    public void Initialize()
    {
        _FileItems = _fileListService.FileInfoList();
        SetOutputDirectory();
    }

    public void PerformAction()
    {
        List<FileInfoModel> paths = new();
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
                    FileHelp.CopyFile(item.Path, "C:\\Users\\tangx\\Downloads\\新建文件夹");
                }
            }
        }
    }

    //私有方法
    private void SetOutputDirectory()
    {
        if (string.IsNullOrWhiteSpace(GlobalConfig.config.OutputDirectory))
        {
            _DownloadsPath = _fileListService.GetDownloadsPath();
            GlobalConfig.config.OutputDirectory = _DownloadsPath;
        }
        else
        {
            _DownloadsPath = GlobalConfig.config.OutputDirectory;
        }
    }
}