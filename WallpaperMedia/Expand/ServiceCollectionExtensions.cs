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
        collection.AddSingleton<MainWindowViewModel>(); //注册窗口
        collection.AddSingleton<IFileListService, FileListService>();
        collection.AddScoped<RegeditHelp>((a) => new RegeditHelp(GlobalConfig.SteamRegedit));
        collection.AddSingleton<IRePKGService, RePKGService>();
    }
}