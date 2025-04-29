using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SystemProgramming6;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static Mutex mutex = new Mutex(true, "MutexUniqueTest");

    public MainWindow()
    {
        InitializeComponent();

        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            MessageBox.Show("\n App is running",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            Application.Current.Shutdown();
            return;
        }
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            RunApplication();
        }
        finally
        {
            ;
        }
    }

    private void RunApplication()
    {
        MessageBox.Show($"App is sucessfuly started (PID: {System.Diagnostics.Process.GetCurrentProcess().Id})\n" +
                        "This prod can only start with one copy.",
            "Information about prod: ", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        mutex.ReleaseMutex();
        mutex.Dispose();
    }
}