using System.Text.Json.Serialization;

namespace WallpaperMedia.Models.FileListService;

[JsonSerializable(typeof(FileInfoJson))]
public partial class FileInfoJsonContext : JsonSerializerContext
{
}
public class FileInfoJson
{
    public string title { get; set; }
    public string preview { get; set; }
    public string file { get; set; }
    public string type { get; set; }
}