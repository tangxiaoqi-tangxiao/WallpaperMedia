using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WallpaperMedia.Expand;
using WallpaperMedia.Utils;
using WallpaperMedia.ViewModels;
using WallpaperMedia.Views;

namespace WallpaperMedia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //读取配置文件
        FileHelp.ReadConfig();
        // 注册应用程序运行所需的所有服务
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        // 从 collection 提供的 IServiceCollection 中创建包含服务的 ServiceProvider
        var services = collection.BuildServiceProvider();

        var vm = services.GetRequiredService<MainWindowViewModel>();
        //检测是否是桌面平台
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
            //监听关闭请求事件
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
        }
        //检测是否是平板或手机
        // else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        // {
        //     singleViewPlatform.MainView = new MainWindow
        //     {
        //         DataContext = vm
        //     };
        // }
        
        //执行父类方法
        base.OnFrameworkInitializationCompleted();
    }
    
    private bool _canClose; //此标志用于检查是否允许窗口关闭
    private void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        e.Cancel = !_canClose; //第一次取消关闭事件

        if (!_canClose)
        { 
            FileHelp.WriteConfig();
            //将_canClose设置为true并再次关闭此窗口
            _canClose = true;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}