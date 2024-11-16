using System.Collections.ObjectModel;
using WallpaperMedia.ViewModels.MainModels;

namespace WallpaperMedia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    public string FileName => "文件名称";
    public string Size => "文件大小";
    public string Resolution => "分辨率";
    public string Status => "状态";
    public string DeleteCommand => "删除";
#pragma warning restore CA1822 // Mark members as static

}