namespace MusicStore.Models;

public class Album
{
    public string CoverUrl { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }

    public Album(string artist, string title, string coverUrl)
    {
        Artist = artist;
        Title = title;
        CoverUrl = coverUrl;
    }
    
    
    
}