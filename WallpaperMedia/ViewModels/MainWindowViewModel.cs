using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using WallpaperMedia.Configs;
using WallpaperMedia.CustomException;
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
    
    private bool _widgetIsButtonEnabled = true;
    private bool _widgetSteamSelectorIsVisible = true;
    public bool WidgetIsButtonEnabled
    {
        get => _widgetIsButtonEnabled;
        set
        {
            _widgetIsButtonEnabled = value;
            //更新UI
            OnPropertyChanged();
        }
    }

    public bool WidgetSteamSelectorIsVisible
    {
        get => _widgetSteamSelectorIsVisible;
        set
        {
            _widgetSteamSelectorIsVisible = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 获取<see cref="ToDoItem"/>的集合，该集合允许添加和删除项目
    /// </summary>
    public List<FileInfoModel> _FileItems { get; set; } = new();

    public void Initialize()
    {
        try
        {
            _FileItems = _fileListService.FileInfoList();
        }
        catch (WallpaperPathError e)
        {
            if (e.ErrorType == WallpaperPathErrorEnum.Steam)
            {
                ShowSteamSelector(true);
            }
            else
            {
                ShowSteamSelector(false);
            }
        }
        
        //设置输出路径
        SetOutputDirectory();
    }

    //导出命令
    public async void ExportOriginalFile()
    {
        try
        {
            WidgetIsButtonEnabled = false;
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
                    string fileName = FileHelp.CleanFileName(item.Title);
                    if (fileName.Length > 20)
                        fileName = fileName.Substring(0, 20);
                    if (item.IsProcess)
                    {
                        await _rePKGService.ExtractFile(item.Path, fileName);
                    }
                    else
                    {
                        string output = Path.Combine(GlobalConfig.config.OutputDirectory, fileName);
                        Directory.CreateDirectory(output);
                        FileHelp.CopyFile(item.Path, output);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            WidgetIsButtonEnabled = true;
        }
    }

    //私有方法
    private void SetOutputDirectory()
    {
        if (string.IsNullOrWhiteSpace(GlobalConfig.config.OutputDirectory))
        {
            GlobalConfig.config.OutputDirectory = _fileListService.GetDownloadsPath();
        }
    }

    void ShowSteamSelector(bool isShow)
    {
        WidgetSteamSelectorIsVisible = true;
    }
}