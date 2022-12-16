using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MusicStore.Views;

public partial class MusicStoreWindow : Window
{
    public MusicStoreWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}