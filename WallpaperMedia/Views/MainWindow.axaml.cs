using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using WallpaperMedia.Configs;
using WallpaperMedia.Services;
using WallpaperMedia.ViewModels;
using WallpaperMedia.ViewModels.MainModels;

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

    public void Initialize()
    {
        const int minWidth = 840;
        const int minHeight = 500;
        //设置窗口大小
        PixelRect screen = Screens.Primary?.WorkingArea ?? new PixelRect(new PixelSize(minWidth, minHeight));
        int elasticWidth = screen.Width / 3;
        int elasticHeight = screen.Height / 3;

        int width = elasticWidth > minWidth ? elasticWidth : minWidth;
        int height = elasticHeight > minHeight ? elasticHeight : minHeight;

        this.Width = width;
        this.Height = height;
        this.MinWidth = minWidth;
        this.MinHeight = minHeight;

        InitializeEvent();
    }

    //初始化事件
    private void InitializeEvent()
    {
        // 监听窗口大小变化事件
        this.SizeChanged += OnSizeChanged;
        this.Loaded += MainWindow_Loaded;
        OutputDirectory.TextChanged += OnTextChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 在这里处理ViewModel初始化完成后的逻辑
        BuildContentComponent();
        OutputDirectory.Text = _ViewModel._DownloadsPath;
    }

    //监听窗口大小变化
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var newWidth = e.NewSize.Width;
        // 使用 FindControl 确保获取到控件
        if (GridContentColumn == null)
            return;
        // 动态调整列数
        if (newWidth <= ScreenSize.Md)
        {
            GridContentColumn.Columns = 3;
        }
        else if (newWidth <= ScreenSize.Lg)
        {
            GridContentColumn.Columns = 4;
        }
        else if (newWidth <= ScreenSize.Xl)
        {
            GridContentColumn.Columns = 5;
        }
        else if (newWidth <= ScreenSize.Xxl)
        {
            GridContentColumn.Columns = 6;
        }
        else if (newWidth <= ScreenSize.Xxxxl)
        {
            GridContentColumn.Columns = 7;
        }
        else
        {
            GridContentColumn.Columns = 8;
        }
    }

    //构建内容组件
    private void BuildContentComponent()
    {
        GridContentColumn.Children.Clear();
        foreach (var viewModelFileItem in _ViewModel._FileItems)
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

            GridContentColumn.Children.Add(border);
        }
    }

    //监听文本框更改
    private void OnTextChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.Text = _ViewModel._DownloadsPath;
        }
    }
}