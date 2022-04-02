using System.Linq;
using System.ServiceProcess;
using System.Windows;
using Microsoft.Win32;

namespace ServiceHandler.WPF;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ServiceController _serviceController;
    private readonly ServiceManager _serviceManager;

    public MainWindow()
    {
        InitializeComponent();
        _serviceController = new ServiceController();
        _serviceManager = new ServiceManager();
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        _serviceManager.AddService(ServiceNameTextBox.Text, LocalPathTextBox.Text);

        ServiceListView.Items.Add(ServiceNameTextBox.Text);
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _serviceController.ServiceName =
            _serviceManager
                .GetServices()
                .FirstOrDefault(x => x.EndsWith(name))!;

        if (_serviceController.Status == ServiceControllerStatus.Stopped)
        {
            _serviceController.Start();
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _serviceController.ServiceName =
            _serviceManager
                .GetServices()
                .FirstOrDefault(x => x.EndsWith(name))!;

        if (_serviceController.Status == ServiceControllerStatus.Running)
        {
            _serviceController.Stop();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        _serviceController.ServiceName =
            _serviceManager
                .GetServices()
                .FirstOrDefault(x => x.EndsWith(name))!;

        if (_serviceController.Status == ServiceControllerStatus.Running)
        {
            _serviceController.Stop();
        }

        ServiceListView.Items.Remove(name);

        _serviceManager.RemoveService(name);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ServiceListView.Items.Clear();
        ServiceListView.ItemsSource = _serviceManager.GetServices();
    }

    private void BrowseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "*.exe|*.dll",
            InitialDirectory = "c:"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LocalPathTextBox.Text = openFileDialog.FileName;
        }
    }
}