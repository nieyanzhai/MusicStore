using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Threading;
using MusicStore.Services;
using ReactiveUI;

namespace MusicStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AlbumFileService _albumFileService;
        private bool _isAlbumEmpty;

        public bool IsAlbumEmpty
        {
            get => _isAlbumEmpty;
            set => this.RaiseAndSetIfChanged(ref _isAlbumEmpty, value);
        }

        public ObservableCollection<AlbumViewModel> Album { get; } = new();

        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
        public ICommand BuyMusicCommand { get; }

        public MainWindowViewModel()
        {
            _albumFileService = new AlbumFileService();

            this.WhenAnyValue(x => x.Album.Count)
                .Select(x => x == 0)
                .Subscribe(x => IsAlbumEmpty = x);


            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();
            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel();
                var result = await ShowDialog.Handle(store);
                if (result != null)
                {
                    Album.Add(result);
                    await result.SaveToDisk();
                }
            });

            Dispatcher.UIThread.InvokeAsync(LoadAlbumFromDisk);
        }


        private async Task LoadAlbumFromDisk()
        {
            Album.Clear();
            foreach (var album in await _albumFileService.LoadAlbumFromDisk("./Cache"))
            {
                if (album != null) Album.Add(new AlbumViewModel(album));
            }

            foreach (var album in Album)
            {
                await album.LoadCoverAsync();
            }
        }
    }
}