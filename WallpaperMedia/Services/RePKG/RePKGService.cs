using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RePKG.Application.Package;
using RePKG.Application.Texture;
using RePKG.Core.Package;
using RePKG.Core.Package.Enums;
using RePKG.Core.Package.Interfaces;
using RePKG.Core.Texture;
using WallpaperMedia.Configs;

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

    public Task ExtractFile(string filePath, string fileName)
    {
        return Task.Run(() =>
        {
            if (!File.Exists(filePath))
                return;

            ExtractPkg(new FileInfo(filePath), fileName);
        });
    }

    private void ExtractPkg(FileInfo file, string fileName)
    {
        Console.WriteLine($"\r\n### Extracting package: {file.FullName}");

        // Load package
        Package package;

        using (var reader = new BinaryReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
            package = _packageReader.ReadFrom(reader);
        }

        // Extract package entries
        foreach (var entry in package.Entries)
        {
            ExtractEntry(entry, Path.Combine(GlobalConfig.config.OutputDirectory, fileName));
        }
    }

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    private void ExtractEntry(PackageEntry entry, string outputDirectory)
    {
        // save raw
        // var filePathWithoutExtension = Path.Combine(outputDirectory, entry.DirectoryPath, entry.Name);
        //
        // var filePath = filePathWithoutExtension + entry.Extension;
        //
        // Directory.CreateDirectory(Path.GetDirectoryName(filePathWithoutExtension));
        //
        // if (File.Exists(filePath))
        //     Console.WriteLine($"* Skipping, already exists: {filePath}");
        // else
        // {
        //     Console.WriteLine($"* Extracting: {entry.FullPath}");
        //
        //     File.WriteAllBytes(filePath, entry.Bytes);
        // }

        // convert and save
        if (entry.Type != EntryType.Tex)
            return;

        var tex = LoadTex(entry.Bytes, entry.FullPath);

        if (tex == null)
            return;

        try
        {
            string filePathWithoutExtension = entry.DirectoryPath == "materials"
                ? Path.Combine(outputDirectory, entry.Name)
                : Path.Combine(outputDirectory, entry.DirectoryPath, entry.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(filePathWithoutExtension));
            ConvertToImageAndSave(tex, filePathWithoutExtension, false);
            // var jsonInfo = _texJsonInfoGenerator.GenerateInfo(tex);
            // File.WriteAllText($"{filePathWithoutExtension}.tex-json", jsonInfo);
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