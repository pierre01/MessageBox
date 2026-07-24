using MessageBox.Wpf.Sample.ViewModels;
using System.Windows;

namespace MessageBox.Wpf.Sample.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
