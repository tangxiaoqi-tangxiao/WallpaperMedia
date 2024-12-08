using System;

namespace WallpaperMedia.CustomException;

public class WallpaperPathError : Exception
{
    public WallpaperPathErrorEnum ErrorType = WallpaperPathErrorEnum.NoError;

    // 接受错误消息的构造函数
    public WallpaperPathError(string message, WallpaperPathErrorEnum errorType) : base(message)
    {
        ErrorType = errorType;
    }
}

public enum WallpaperPathErrorEnum
{
    NoError = 0,
    Steam = 1,
    Wallpapers = 2,
}