using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using WallpaperMedia.Configs;
using WallpaperMedia.CustomException;
using WallpaperMedia.Models.FileListService;
using WallpaperMedia.ViewModels;

namespace WallpaperMedia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        // 必须初始化XAML中的控件
        InitializeComponent();
        //初始化
        Initialize();
    }

    private MainWindowViewModel _ViewModel => DataContext as MainWindowViewModel;
    private bool _isLoading;
    private DispatcherTimer _resizeTimer;
    private int _CurrentWidth = 0;
    private int _CurrentHeight = 0;

    public void Initialize()
    {
        //初始化窗口
        InitializeMainWindow();

        //设置Grid列数量
        SetGridColumns((int)this.Width);

        //初始化事件
        InitializeEvent();

        _resizeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500) // 延迟 500 毫秒
        };
        _resizeTimer.Tick += OnResizeTimerTick;
    }

    //初始化事件
    private void InitializeEvent()
    {
        // 监听窗口大小变化事件
        this.SizeChanged += OnSizeChanged;
        // 加载完成
        this.Loaded += MainWindow_Loaded;
        WidgetOutputDirectory.TextChanged += OnTextChanged;
    }

    //窗口加载完成执行事件
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            //初始化ViewModel
            _ViewModel.Initialize();

            // 加载输出文件路径
            WidgetOutputDirectory.Text = GlobalConfig.config.OutputDirectory;
            
            _isLoading = true;
            
            //没有数据展示提示词
            if (_ViewModel._FileItems.Count == 0)
                ShowHint(true);
            
            //加载数据
            BuildContentComponent(_ViewModel._FileItems);
        }
        catch (WallpaperPathError mError)
        {
            if (mError.ErrorType == WallpaperPathErrorEnum.Steam)
            {
                ShowSteamSelector(true);
            }
        }
    }

    //监听窗口大小变化
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // 每次大小变化时重置定时器
        _resizeTimer.Stop();
        _CurrentWidth = (int)e.NewSize.Width;
        _CurrentHeight = (int)e.NewSize.Height;
        _resizeTimer.Start();
    }

    //构建内容组件
    private void BuildContentComponent(List<FileInfoModel> fileList)
    {
        WidgetGridContentColumn.Children.Clear();
        foreach (var viewModelFileItem in fileList)
        {
            Border border = new();
            Image image = new();
            Grid grid = new();
            TextBlock textBlock = new();
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(10);
            border.Background = new SolidColorBrush(Color.Parse("#333")); //自定义颜色#333
            border.ClipToBounds = true; // 超出部分裁剪
            border.PointerPressed += (sender, e) =>
            {
                // 获取鼠标按键状态
                var pointerPoint = e.GetCurrentPoint(border);
                if (pointerPoint.Properties.IsLeftButtonPressed) // 判断是否为左键单击
                {
                    // 单击事件逻辑：切换边框颜色
                    if (border.BorderBrush == null || border.BorderBrush.Equals(Brushes.Transparent))
                    {
                        border.BorderBrush = new SolidColorBrush(Color.Parse("#4183f5")); // 设置边框为蓝色
                        border.BorderThickness = new Thickness(2); // 设置边框宽度
                        viewModelFileItem.Selected = true;
                    }
                    else
                    {
                        border.BorderBrush = Brushes.Transparent; // 取消边框颜色
                        border.BorderThickness = new Thickness(0);
                        viewModelFileItem.Selected = false;
                    }
                }

                // 在这里处理单击事件的逻辑
                Console.WriteLine($"Clicked on: {viewModelFileItem.Title}");
                // 可以在这里执行任何其他操作，例如导航、弹出消息等
            };

            border.Child = grid;

            image.Stretch = Stretch.UniformToFill;
            image.Source = viewModelFileItem.ThumbnailBitmap;
            image.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative); // 以中心为基点
            grid.Children.Add(image);

            textBlock.Text = viewModelFileItem.Title;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;
            textBlock.Foreground = Brushes.White;
            textBlock.FontSize = 14;
            textBlock.Background = new SolidColorBrush(Color.Parse("#99000000")); //自定义颜色#99000000
            textBlock.Padding = new Thickness(5);
            grid.Children.Add(textBlock);

            WidgetGridContentColumn.Children.Add(border);
        }
    }

    //监听文本框更改
    private void OnTextChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.Text = GlobalConfig.config.OutputDirectory;
        }
    }

    //延迟触发窗口变化事件
    private void OnResizeTimerTick(object sender, EventArgs e)
    {
        // 处理最后一次触发的事件
        _resizeTimer.Stop();
        PixelRect screen = Screens.Primary.WorkingArea;
        //判断全屏
        if (screen.Width != _CurrentWidth)
        {
            GlobalConfig.config.Width = _CurrentWidth;
            GlobalConfig.config.Height = _CurrentHeight;
        }

        SetGridColumns(_CurrentWidth);
    }

    //设置内容元素列数量
    private void SetGridColumns(int newWidth)
    {
        // 使用 FindControl 确保获取到控件
        if (WidgetGridContentColumn == null)
            return;
        // 使用一个映射数组来存储屏幕宽度对应的列数
        var widthToColumns = new (int maxWidth, int columns)[]
        {
            (ScreenSize.Md, 3),
            (ScreenSize.Lg, 4),
            (ScreenSize.Xl, 5),
            (ScreenSize.Xxl, 6),
            (ScreenSize.Xxxxl, 7),
        };
        // 使用 LINQ 查找匹配的列数
        var matchingColumn = widthToColumns.FirstOrDefault(mapping => newWidth <= mapping.maxWidth);
        // 如果找到了匹配的列数，则设置，否则默认设置为 8
        WidgetGridContentColumn.Columns = matchingColumn.columns != 0 ? matchingColumn.columns : 8;
    }

    //初始化窗口
    private void InitializeMainWindow()
    {
        const int minWidth = 840;
        const int minHeight = 500;

        //设置窗口大小
        PixelRect screen = Screens.Primary?.WorkingArea ?? new PixelRect(new PixelSize(minWidth, minHeight));
        int elasticWidth = screen.Width / 3;
        int elasticHeight = screen.Height / 3;

        int width = elasticWidth > minWidth ? elasticWidth : minWidth;
        int height = elasticHeight > minHeight ? elasticHeight : minHeight;

        if (GlobalConfig.config.Width > 0 && GlobalConfig.config.Width >= minWidth)
        {
            this.Width = GlobalConfig.config.Width;
        }
        else
        {
            this.Width = width;
        }

        if (GlobalConfig.config.Height > 0 && GlobalConfig.config.Height >= minHeight)
        {
            this.Height = GlobalConfig.config.Height;
        }
        else
        {
            this.Height = height;
        }

        this.MinWidth = minWidth;
        this.MinHeight = minHeight;
    }

    //打开文件选择器
    private void FolderSelector(object? sender, RoutedEventArgs e)
    {
        var downloads = StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads).Result;

        var folder = StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "导出目录",
            SuggestedStartLocation = downloads
        }).Result;
        if (folder.Count > 0)
        {
            GlobalConfig.config.OutputDirectory = folder[0].Path.LocalPath;
            WidgetOutputDirectory.Text = GlobalConfig.config.OutputDirectory;
        }
    }
    
    //打开Steam文件选择器
    private void SteamFolderSelector(object? sender, RoutedEventArgs e)
    {
        var folder = StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Steam目录"
        }).Result;
        
        if (folder.Count > 0)
        {
            string folderPath = Path.Combine(folder[0].Path.LocalPath,"steam.exe");
            //判断该文件夹是否存在Steam
            if (File.Exists(folderPath))
            {
                GlobalConfig.config.SteamPath = folderPath;
            }
            else
            {
                Console.WriteLine("文件不存在。");
            }
        }
    }

    // 处理 ComboBox 的选择事件
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        var selectedItem = comboBox.SelectedItem as ComboBoxItem;

        // 显示选择的项
        if (selectedItem != null && _isLoading)
        {
            List<FileInfoModel> fileList = null;
            var selectedIndex = (string)selectedItem.Tag;
            if (selectedIndex == "0")
            {
                fileList = _ViewModel._FileItems;
            }
            else if (selectedIndex == "1")
            {
                fileList = _ViewModel._FileItems.Where(e => e.IsProcess).ToList();
            }
            else
            {
                fileList = _ViewModel._FileItems.Where(e => !e.IsProcess).ToList();
            }

            //重新渲染内容
            BuildContentComponent(fileList);
        }
    }

    //搜索
    private void Search(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (!string.IsNullOrWhiteSpace(WidgetValue.Text) && (string)WidgetSearchButton.Content! == "搜索")
        {
            WidgetSearchButton.Content = "清除";
            var list = _ViewModel._FileItems.Where(e => e.Title != null && e.Title.Contains(WidgetValue.Text)).ToList();
            if (list.Count == 0)
                ShowHint(true);
            else
                ShowHint(false);
            BuildContentComponent(list);
        }
        else
        {
            WidgetSearchButton.Content = "搜索";
            WidgetValue.Text = "";
            ShowHint(false);
            BuildContentComponent(_ViewModel._FileItems);
        }
    }

    //显示提示
    private void ShowHint(bool isShow)
    {
        WidgetGridContentColumn.IsVisible = !isShow;
        WidgetHint.IsVisible = isShow;

        WidgetStackPanelContent.Orientation = !isShow ? Orientation.Vertical : Orientation.Horizontal;
        WidgetStackPanelContent.HorizontalAlignment = HorizontalAlignment.Center;
    }

    //显示
    private void ShowSteamSelector(bool isShow)
    {
        //显示控件
        WidgetSteamSelector.IsVisible = isShow;
        if (isShow)
        {
            IsEnabledComponent(false);
        }
        else
        {
            IsEnabledComponent(true);
        }
        
        WidgetStackPanelContent.Orientation = !isShow ? Orientation.Vertical : Orientation.Horizontal;
        WidgetStackPanelContent.HorizontalAlignment = HorizontalAlignment.Center;

        //隐藏控件
        WidgetGridContentColumn.IsVisible = !isShow;
        WidgetHint.IsVisible = !isShow;
    }
    
    //启用或禁用组件
    private void IsEnabledComponent(bool isEnabled)
    {
        WidgetExportButton.IsEnabled = isEnabled;
        WidgetSearchButton.IsEnabled = isEnabled;
        ComboBox.IsEnabled = isEnabled;
    }
}