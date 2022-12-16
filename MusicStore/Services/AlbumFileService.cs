using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MusicStore.Models;
using RestSharp;

namespace MusicStore.Services;

public class AlbumFileService
{
    private const int CoverWidth = 400;


    public async Task SaveAlbumToDisk(string path, Album album)
    {
        EnsureDirectoryExist(path);
        await using var fs = File.OpenWrite(path);
        await JsonSerializer.SerializeAsync(fs, album);
    }


    public async Task<IEnumerable<Album?>> LoadAlbumFromDisk(string path)
    {
        EnsureDirectoryExist(path);

        var albums = new List<Album>();
        foreach (var file in Directory.EnumerateFiles(path))
        {
            if (!string.IsNullOrEmpty(new FileInfo(file).Extension)) continue;
            await using var fs = File.OpenRead(file);
            albums.Add(await JsonSerializer.DeserializeAsync<Album>(fs));
        }

        return albums;
    }


    public async Task<Bitmap?> LoadCoverAsync(string path, string coverUrl)
    {
        var fileName = $"{path}.bmp";
        if (File.Exists(fileName)) return LoadCoverFromDisk(fileName);

        using var restClient = new RestClient(coverUrl);
        var bytes = await restClient.DownloadDataAsync(new RestRequest());
        if (bytes == null) return null;

        // Save to disk
        // await SaveCoverToDisk(fileName, bytes);
        return Bitmap.DecodeToWidth(new MemoryStream(bytes), CoverWidth);
    }


    public void SaveCoverToDisk(string path, Bitmap? img)
    {
        if (img == null) return;
        EnsureDirectoryExist(path);
        var fileName = $"{path}.bmp";
        if (File.Exists(fileName)) File.Delete(fileName);

        // Save image to disk
        using var fs = File.OpenWrite(fileName);
        img.Save(fs);
    }


    private Bitmap LoadCoverFromDisk(string path)
    {
        var stream = new MemoryStream(File.ReadAllBytes(path));
        return Bitmap.DecodeToWidth(stream, CoverWidth);
    }

    private void EnsureDirectoryExist(string path)
    {
        var dirName = Path.GetDirectoryName(path);
        if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
    }
}