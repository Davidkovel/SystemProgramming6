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
    private Semaphore semaphore = new Semaphore(3, 3);
    private Random random = new Random();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartThreads_Click(object sender, RoutedEventArgs e)
    {
        outputTextBox.Clear();
        startButton.IsEnabled = false;

        for (int i = 0; i < 10; i++)
        {
            int threadNumber = i + 1;
            Thread thread = new Thread(() => RunThread(threadNumber));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void RunThread(int threadNumber)
    {
        semaphore.WaitOne();

        try
        {
            Dispatcher.Invoke(() =>
                outputTextBox.AppendText(
                    $"Thread {threadNumber} (ID: {Thread.CurrentThread.ManagedThreadId}) started...\n"));

            for (int j = 0; j < 5; j++)
            {
                int number = random.Next(1, 100);
                Dispatcher.Invoke(() =>
                    outputTextBox.AppendText($"Thread {threadNumber}: {number}\n"));
                Thread.Sleep(random.Next(200, 500));
            }

            Dispatcher.Invoke(() =>
                outputTextBox.AppendText($"Thread {threadNumber} finished.\n"));
        }
        finally
        {
            semaphore.Release();
            Dispatcher.Invoke(() =>
            {
                if (semaphore.WaitOne(0))
                {
                    semaphore.Release();
                    startButton.IsEnabled = true;
                }
            });
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        semaphore.Dispose();
        base.OnClosed(e);
    }
}