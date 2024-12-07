using System;
using System.IO;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WallpaperMedia.Configs;

namespace WallpaperMedia.Utils;

public class FileHelp
{
    //拷贝文件
    public static bool CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
    {
        // 确保源文件存在
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("源文件不存在", sourcePath);
        }

        // 确保目标目录存在
        if (!Directory.Exists(destinationPath))
        {
            throw new DirectoryNotFoundException("目标目录不存在");
        }

        string filePath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));

        // 检查目标文件是否已经存在

        if (!File.Exists(filePath))
        {
            // 如果文件不存在，则复制文件
            File.Copy(sourcePath, filePath, false);
            Console.WriteLine("文件已成功复制。");
            return true;
        }

        return false;
    }
    
    //处理文件夹名称非法字符
    public static string CleanFileName(string input)
    {
        // 定义 Windows 文件夹名称中的非法字符
        string invalidChars = @"[<>:""/\\|?*]";

        // 去除非法字符
        string cleanName = Regex.Replace(input, invalidChars, "");

        return cleanName;
    }

    //读取配置
    public static void ReadConfig()
    {
        try
        {
            using var fs = File.OpenRead(FileConfig.ConfigPath);
            if (fs.Length != 0)
                GlobalConfig.config = JsonSerializer.DeserializeAsync(fs, ConfigContext.Default.Config).Result;
            else
            {
                GlobalConfig.config = new Config();
            }
        }
        catch (Exception e)
        {
            GlobalConfig.config = new Config();
        }
    }

    //写入配置
    public static void WriteConfig()
    {
        if (GlobalConfig.config != null)
        {
            using var fs = File.Create(FileConfig.ConfigPath);
            JsonSerializer.SerializeAsync(fs, GlobalConfig.config, ConfigContext.Default.Config).Wait();
        }
    }
}