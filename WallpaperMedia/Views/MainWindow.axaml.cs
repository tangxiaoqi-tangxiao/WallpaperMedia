using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using WallpaperMedia.Services;
using WallpaperMedia.ViewModels.MainModels;

namespace WallpaperMedia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        //初始化
        Initialize();
    }

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
    }

    private void OnGridLoaded(object sender, VisualTreeAttachmentEventArgs e)
    {
        // 确保事件只执行一次
        if (sender is Grid grid)
        {
            // 添加标题行
            AddHeaderRow(grid);

            // 自动添加10行数据
            for (int i = 0; i < 100; i++)
            {
                AddRow(i + 1, grid);
            }
        }
    }

    private void AddHeaderRow(Grid grid)
    {
        // 添加标题行
        var titles = new[] { "Title 1", "Title 2", "Title 3", "Title 4" };
        for (int i = 0; i < titles.Length; i++)
        {
            var header = new TextBlock
            {
                Text = titles[i],
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Margin = new Thickness(5),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            Grid.SetColumn(header, i);
            Grid.SetRow(header, 0);
            grid.Children.Add(header);
        }

        // 添加行定义
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    }

    private void AddRow(int index, Grid grid)
    {
        // 添加新行定义
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // 在该行添加四列数据
        for (int i = 0; i < 4; i++)
        {
            var cell = new TextBlock
            {
                Text = $"Row {index}, Col {i + 1}",
                Margin = new Thickness(5),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            Grid.SetColumn(cell, i);
            Grid.SetRow(cell, index);
            grid.Children.Add(cell);
        }
    }
}