using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MusicStore.Models;
using MusicStore.Services;
using ReactiveUI;
using RestSharp;

namespace MusicStore.ViewModels;

public class AlbumViewModel : ViewModelBase
{
    private string CachePath => $"./Cache/{Artist} - {Title}";
    private readonly AlbumFileService _albumFileService;
    
    private Album _album;

    private Bitmap? _cover;

    public Bitmap? Cover
    {
        get => _cover;
        set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    public string Title => _album.Title;

    public string Artist => _album.Artist;

    public AlbumViewModel(Album album)
    {
        _albumFileService = new AlbumFileService();
        _album = album;
    }

    public async Task LoadCoverAsync() => Cover = await _albumFileService.LoadCoverAsync(CachePath, _album.CoverUrl);
    
    public async Task SaveToDisk()
    {
        _albumFileService.SaveCoverToDisk(CachePath, Cover);
        await _albumFileService.SaveAlbumToDisk(CachePath, _album);
    }
}