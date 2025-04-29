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
    private Semaphore semaphore = new Semaphore(3, 3); // Максимум 3 потоки одночасно
    private Random random = new Random();
    private TextBox outputTextBox;

    public MainWindow()
    {
        InitializeComponent();
        CreateUI();
    }

    private void CreateUI()
    {
        var stackPanel = new StackPanel();

        var startButton = new Button
        {
            Content = "10 Threads",
            Margin = new Thickness(10),
            Padding = new Thickness(5)
        };
        startButton.Click += StartThreads_Click;

        outputTextBox = new TextBox
        {
            Margin = new Thickness(10),
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            TextWrapping = TextWrapping.Wrap,
            IsReadOnly = true,
            Height = 300
        };

        stackPanel.Children.Add(startButton);
        stackPanel.Children.Add(outputTextBox);
        this.Content = stackPanel;
    }

    private void StartThreads_Click(object sender, RoutedEventArgs e)
    {
        outputTextBox.Clear();

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
                    $"Thread {threadNumber} (ID: {Thread.CurrentThread.ManagedThreadId}) has started..\n"));

            for (int j = 0; j < 5; j++)
            {
                int number = random.Next(1, 100);
                Dispatcher.Invoke(() =>
                    outputTextBox.AppendText($"Thread {threadNumber}: {number}\n"));
                Thread.Sleep(random.Next(200, 800));
            }

            Dispatcher.Invoke(() =>
                outputTextBox.AppendText($"Thread {threadNumber} has ended\n"));
        }
        finally
        {
            semaphore.Release();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        semaphore.Dispose();
        base.OnClosed(e);
    }
}