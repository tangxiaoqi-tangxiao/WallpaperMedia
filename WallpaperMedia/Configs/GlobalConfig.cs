using System.Text.Json.Serialization;
using WallpaperMedia.Models.FileListService;

namespace WallpaperMedia.Configs;

public class GlobalConfig
{
    public static Config config { get; set; }
}

[JsonSerializable(typeof(Config))]
public partial class ConfigContext : JsonSerializerContext
{
}
public class Config
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string OutputDirectory { get; set; }
}