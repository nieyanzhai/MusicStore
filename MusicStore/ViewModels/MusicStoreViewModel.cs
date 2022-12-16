using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicStore.Models;
using MusicStore.Services;
using ReactiveUI;

namespace MusicStore.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private CancellationTokenSource _cancellationTokenSource;

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    private string? _searchText;

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private AlbumViewModel? _selectedAlbum;
    private readonly ItunesService _itunesService;

    public AlbumViewModel? SelectedAlbum
    {
        get => _selectedAlbum;
        set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
    }

    public ObservableCollection<AlbumViewModel?> SearchResults { get; } = new();

    public ReactiveCommand<Unit, AlbumViewModel?> BuyAlbumCommand { get; }

    public MusicStoreViewModel()
    {
        _itunesService = new ItunesService();

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async searchText => await DoSearch(searchText));

        BuyAlbumCommand = ReactiveCommand.Create(() => SelectedAlbum);
    }

    private async Task DoSearch(string? searchText)
    {
        IsBusy = true;
        SearchResults.Clear();

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var albums = await _itunesService.SearchAsync(searchText);

            foreach (var album in albums)
            {
                SearchResults.Add(new AlbumViewModel(album));
            }

            if (!cancellationToken.IsCancellationRequested) await LoadCover(cancellationToken);
        }

        IsBusy = false;
    }

    private async Task LoadCover(CancellationToken cancellationToken)
    {
        var results = SearchResults.ToList();
        foreach (var albumViewModel in results.Where(albumViewModel => albumViewModel != null))
        {
            await albumViewModel!.LoadCoverAsync();
            if (cancellationToken.IsCancellationRequested) break;
        }
    }
}