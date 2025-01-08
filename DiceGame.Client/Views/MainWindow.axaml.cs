using Avalonia.Controls;
using BakDiceClient.ViewModels;

namespace BakDiceClient.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}