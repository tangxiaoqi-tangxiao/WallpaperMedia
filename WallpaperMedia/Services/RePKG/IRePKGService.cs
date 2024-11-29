using System.Threading.Tasks;

namespace WallpaperMedia.Services.RePKG;

public interface IRePKGService
{
    Task ExtractFile(string filePath,string fileName);
}