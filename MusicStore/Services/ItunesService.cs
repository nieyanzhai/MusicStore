using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTunesSearch.Library;
using MusicStore.Models;

namespace MusicStore.Services;

public class ItunesSearchService
{
    private readonly iTunesSearchManager _iTunesSearchManager;

    public ItunesSearchService()
    {
        _iTunesSearchManager = new iTunesSearchManager();
    }
    
    public async Task<IEnumerable<Album>?> SearchAsync(string searchTerm)
    {
        var results = await _iTunesSearchManager.GetAlbumsAsync(searchTerm);
        return results?.Albums.Select(result =>
            new Album(result.ArtistName, result.CollectionName, result.ArtworkUrl100));
    }
}