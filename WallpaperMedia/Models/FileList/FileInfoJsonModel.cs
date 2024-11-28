using System.Text.Json.Serialization;

namespace WallpaperMedia.Models.FileListService;

[JsonSerializable(typeof(FileInfoJsonModel))]
public partial class FileInfoJsonModelContext : JsonSerializerContext
{
}
public class FileInfoJsonModel
{
    public string title { get; set; }
    public string preview { get; set; }
    public string file { get; set; }
    public string type { get; set; }
}