using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SystemProgramming6;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Mutex mutex = new Mutex();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        OutputTextBox.Clear();
        StartButton.IsEnabled = false;
        
        Thread thread1 = new Thread(() =>
        {
            mutex.WaitOne();

            for (int i = 0; i <= 20; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputTextBox.AppendText($"Thread 1: {i}\n");
                    OutputTextBox.ScrollToEnd();
                });
                Thread.Sleep(300);
            }

            mutex.ReleaseMutex();
        });

        Thread thread2 = new Thread(() =>
        {
            mutex.WaitOne();

            for (int i = 10; i >= 0; i--)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputTextBox.AppendText($"Thread 2: {i}\n");
                    OutputTextBox.ScrollToEnd();
                });
                Thread.Sleep(300);
            }

            mutex.ReleaseMutex();

            Dispatcher.Invoke(() => StartButton.IsEnabled = true);
        });

        thread1.Start();
        thread2.Start();
    }
}