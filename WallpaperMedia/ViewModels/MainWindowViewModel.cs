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
    
    /// <summary>
    /// 获取<see cref="ToDoItem"/>的集合，该集合允许添加和删除项目
    /// </summary>
    public ObservableCollection<FileInfo> FileItems { get; } = new();

    public void Initialize()
    {
        var fileItems =_fileList.FileInfoList();
        foreach (var item in fileItems)
        {
            FileItems.Add(item);
        }
    }
}