using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using RePKG.Application.Package;
using RePKG.Application.Texture;
using RePKG.Core.Package;
using RePKG.Core.Package.Enums;
using RePKG.Core.Package.Interfaces;
using RePKG.Core.Texture;

namespace WallpaperMedia.Services.RePKG;

public class RePKGService : IRePKGService
{
    private readonly IPackageReader _packageReader;
    private readonly ITexReader _texReader;
    private readonly TexToImageConverter _texToImageConverter;
    private readonly ITexJsonInfoGenerator _texJsonInfoGenerator;
    public RePKGService()
    {
        _packageReader = new PackageReader();
        _texReader = TexReader.Default;
        _texToImageConverter = new TexToImageConverter();
        _texJsonInfoGenerator = new TexJsonInfoGenerator();
    }
    public void ExtractFile(string fileInfo)
    {
        ExtractPkg(new FileInfo(fileInfo));
    }
    private void ExtractPkg(FileInfo file, bool appendFolderName = false, string defaultProjectName = "")
    {
        Console.WriteLine($"\r\n### Extracting package: {file.FullName}");

        // Load package
        Package package;

        using (var reader = new BinaryReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
            package = _packageReader.ReadFrom(reader);
        }

        // Get output directory
        string outputDirectory="C:\\Users\\tangx\\Downloads\\新建文件夹\\";
        var preview = string.Empty;

        // Extract package entries
        foreach (var entry in package.Entries)
        {
            ExtractEntry(entry, ref outputDirectory);
        }
    }
    private static void CopyFiles(IEnumerable<FileInfo> files, string outputDirectory)
    {
        foreach (var file in files)
        {
            var outputPath = Path.Combine(outputDirectory, file.Name);

            if (!false && File.Exists(outputPath))
                Console.WriteLine($"* Skipping, already exists: {outputPath}");
            else
            {
                File.Copy(file.FullName, outputPath, true);
                Console.WriteLine($"* Copying: {file.FullName}");
            }
        }
    }
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    private void ExtractEntry(PackageEntry entry, ref string outputDirectory)
    {
        // save raw
        var filePathWithoutExtension = false
            ? Path.Combine(outputDirectory, entry.Name)
            : Path.Combine(outputDirectory, entry.DirectoryPath, entry.Name);

        var filePath = filePathWithoutExtension + entry.Extension;

        Directory.CreateDirectory(Path.GetDirectoryName(filePathWithoutExtension));

        if (true && File.Exists(filePath))
            Console.WriteLine($"* Skipping, already exists: {filePath}");
        else
        {
            Console.WriteLine($"* Extracting: {entry.FullPath}");

            File.WriteAllBytes(filePath, entry.Bytes);
        }

        // convert and save
        if (false || entry.Type != EntryType.Tex)
            return;

        var tex = LoadTex(entry.Bytes, entry.FullPath);

        if (tex == null)
            return;

        try
        {
            ConvertToImageAndSave(tex, filePathWithoutExtension, false);
            var jsonInfo = _texJsonInfoGenerator.GenerateInfo(tex);
            File.WriteAllText($"{filePathWithoutExtension}.tex-json", jsonInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to write texture");
            Console.WriteLine(e);
        }
    }
    private ITex LoadTex(byte[] bytes, string name)
    {
        Console.WriteLine("* Reading: {0}", name);

        try
        {
            using (var reader = new BinaryReader(new MemoryStream(bytes), Encoding.UTF8))
            {
                return _texReader.ReadFrom(reader);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to read texture");
            Console.WriteLine(e);
        }

        return null;
    }
    private void ConvertToImageAndSave(ITex tex, string path, bool overwrite)
    {
        var format = _texToImageConverter.GetConvertedFormat(tex);
        var outputPath = $"{path}.{format.GetFileExtension()}";

        if (!overwrite && File.Exists(outputPath))
            return;
            
        var resultImage = _texToImageConverter.ConvertToImage(tex);

        File.WriteAllBytes(outputPath, resultImage.Bytes);
    }
}