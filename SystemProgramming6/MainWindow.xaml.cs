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
    private Mutex arrayMutex = new Mutex();
    private int[] dataArray = new int[10];
    private Random random = new Random();

    public MainWindow()
    {
        InitializeComponent();
        GenerateNewArray(); 
        DisplayArray();
    }

    private void GenerateNewArray()
    {
        for (int i = 0; i < dataArray.Length; i++)
        {
            dataArray[i] = random.Next(1, 100);
        }
    }

    private void DisplayArray()
    {
        OutputTextBox.Text = "Current Array:\n";
        OutputTextBox.AppendText(string.Join(", ", dataArray) + "\n\n");
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        GenerateNewArray();
        DisplayArray();
        MaxValueText.Text = "";
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartButton.IsEnabled = false;
        GenerateButton.IsEnabled = false;
        MaxValueText.Text = "";
        OutputTextBox.AppendText("--- Starting calculating ---\n");

        Thread modifierThread = new Thread(() =>
        {
            arrayMutex.WaitOne();

            int increment = random.Next(1, 20);
            Dispatcher.Invoke(() => OutputTextBox.AppendText($"\nThread 1: Adding new element{increment}\n"));

            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] += increment;
                Dispatcher.Invoke(() =>
                {
                    OutputTextBox.AppendText($"Thread 1: Element {i} = {dataArray[i]}\n");
                    OutputTextBox.ScrollToEnd();
                });
                Thread.Sleep(300);
            }

            Dispatcher.Invoke(() => OutputTextBox.AppendText("\nThread 1: Array after modification:\n" +
                                                             string.Join(", ", dataArray) + "\n\n"));

            arrayMutex.ReleaseMutex();
        });

        Thread maxFinderThread = new Thread(() =>
        {
            
            arrayMutex.WaitOne();

            int max = dataArray.Max();
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText($"Thread 2: Maximum Array number = {max}\n");
                MaxValueText.Text = $"Maximum Array number: {max}";
                OutputTextBox.ScrollToEnd();
            });

            arrayMutex.ReleaseMutex();

            Dispatcher.Invoke(() =>
            {
                StartButton.IsEnabled = true;
                GenerateButton.IsEnabled = true;
            });
        });

        modifierThread.Start();
        maxFinderThread.Start();
    }
}