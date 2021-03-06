using System;
using System.Windows;
using Microsoft.Win32;

namespace ServiceHandler.WPF;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ServiceManager _serviceManager;

    public MainWindow()
    {
        InitializeComponent();
        _serviceManager = new ServiceManager();
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _serviceManager.AddService(ServiceNameTextBox.Text, LocalPathTextBox.Text);
            _serviceManager.StartService(ServiceNameTextBox.Text);

            RefreshServicesList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!");
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem?.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        try
        {
            _serviceManager.StartService(name);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!");
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem?.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        try
        {
            _serviceManager.StopService(name);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!");
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ServiceListView.SelectedItem?.ToString();

        if (ServiceListView.Items.Count == 0 || string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        try
        {
            _serviceManager.StopService(name);
            _serviceManager.RemoveService(name);

            RefreshServicesList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!");
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshServicesList();
    }

    private void RefreshServicesList()
    {
        ServiceListView.Items.Clear();
        foreach (var service in _serviceManager.GetServices())
        {
            ServiceListView.Items.Add(service);
        }
    }

    private void BrowseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe|Libraries|*.dll|All Files|*.*",
            InitialDirectory = @"e:\!Programming\Publish\"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LocalPathTextBox.Text = openFileDialog.FileName;
        }
    }
}