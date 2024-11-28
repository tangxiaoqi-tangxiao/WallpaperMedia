using Microsoft.Extensions.DependencyInjection;
using WallpaperMedia.Configs;
using WallpaperMedia.Services;
using WallpaperMedia.Services.FileListService;
using WallpaperMedia.Services.RePKG;
using WallpaperMedia.Utils;
using WallpaperMedia.ViewModels;

namespace WallpaperMedia.Expand;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        //注册窗口
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddScoped<IFileListService, FileListService>();
        collection.AddScoped<IRePKGService, RePKGService>();
    }
}