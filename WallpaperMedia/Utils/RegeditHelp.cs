using Microsoft.Win32;
using System;

namespace WallpaperMedia.Utils;

public class RegeditHelp
{
    private string basePath;

    /// <summary>
    /// 初始化注册表帮助类
    /// </summary>
    /// <param name="basePath">注册表的根路径</param>
    public RegeditHelp(string basePath)
    {
        this.basePath = basePath;
    }

    /// <summary>
    /// 写入注册表值
    /// </summary>
    /// <param name="key">键名称</param>
    /// <param name="valueName">值名称</param>
    /// <param name="value">值内容</param>
    public void Write(string key, string valueName, object value)
    {
        using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey($"{basePath}\\{key}"))
        {
            if (registryKey == null) throw new InvalidOperationException("无法创建或打开注册表键。");
            registryKey.SetValue(valueName, value);
        }
    }

    /// <summary>
    /// 读取注册表值
    /// </summary>
    /// <param name="key">键名称</param>
    /// <param name="valueName">值名称</param>
    /// <returns>值内容</returns>
    public object Read(string key, string valueName)
    {
        string value = string.IsNullOrWhiteSpace(key) ? basePath : $"{basePath}\\{key}";
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(value))
        {
            return registryKey?.GetValue(valueName);
        }
    }

    /// <summary>
    /// 删除注册表值
    /// </summary>
    /// <param name="key">键名称</param>
    /// <param name="valueName">值名称</param>
    public void DeleteValue(string key, string valueName)
    {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey($"{basePath}\\{key}", writable: true))
        {
            registryKey?.DeleteValue(valueName, throwOnMissingValue: false);
        }
    }

    /// <summary>
    /// 删除注册表键
    /// </summary>
    /// <param name="key">键名称</param>
    public void DeleteKey(string key)
    {
        Registry.CurrentUser.DeleteSubKeyTree($"{basePath}\\{key}", throwOnMissingSubKey: false);
    }
}