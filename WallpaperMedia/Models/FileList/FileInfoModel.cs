using Avalonia.Media.Imaging;

namespace WallpaperMedia.Models.FileListService;

public class FileInfoModel
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public required string Path { get; set; }
    
    /// <summary>
    /// 标题
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// 缩略图路径
    /// </summary>
    public string? ThumbnailPath { get; set; }

    public Bitmap? ThumbnailBitmap => ThumbnailPath == null ? null : new Bitmap(ThumbnailPath);

    /// <summary>
    /// 类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 是否需要解包 false不需要 true需要
    /// </summary>
    public bool IsProcess { get; set; }

    /// <summary>
    /// 被选中状态
    /// </summary>
    public bool Selected { get; set; } = false;
}