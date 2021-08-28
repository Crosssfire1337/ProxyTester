using Microsoft.Win32;
using ProxyTester.Proxys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ProxyTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        private List<string> _proxyStringList;
        public readonly List<UseProxy> _proxyList;
        public MainWindow()
        {
            InitializeComponent();
            _openFileDialog = new OpenFileDialog
            {
                Title = "Please choose proxy file"
            };
            _saveFileDialog = new SaveFileDialog
            {
                Title = "Export working proxies",
                DefaultExt = ".txt"
            };

            _proxyList = new List<UseProxy>();

            System.Timers.Timer aTimer = new System.Timers.Timer();

            aTimer.Interval = 1000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                Queue.Content = ThreadPool.PendingWorkItemCount;
            }));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //WPF wants to keep this :)
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() is true)
                //Refactor This
                _proxyStringList = new List<string>(await File.ReadAllLinesAsync(_openFileDialog.FileName));

            using (new Helpers.WaitCursor())
            {
                if (_proxyStringList is not null)
                {
                    try
                    {
                        _proxyStringList.ForEach(prox => _proxyList.Add(new UseProxy(prox, ProxyList, Application.Current.Dispatcher)));
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException || ex is Proxy.InvalidProxyException)
                            throw new Exception("Unknown error");
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_proxyList is not null && _proxyStringList is not null)
            {
                _proxyList.Clear();
                _proxyStringList.Clear();
                ProxyList.Items.Clear();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _proxyList.ForEach(prox => prox.Run(Destination.Text));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            List<string> exportFileLines = new();

            if (_saveFileDialog.ShowDialog() is false)
                return;

            _proxyList.ForEach(prox =>
            {
                if (!(prox.ProxyItem.Status == "success")) return;
                Console.WriteLine(prox.ProxyItem.IP + ":" + prox.ProxyItem.Port.ToString() + ":" + prox.ProxyItem.User + ":" + prox.ProxyItem.Pass);
                exportFileLines.Add(prox.ProxyItem.IP + ":" + prox.ProxyItem.Port.ToString() + ":" + prox.ProxyItem.User + ":" + prox.ProxyItem.Pass);
            });
            File.WriteAllLinesAsync(_saveFileDialog.FileName, exportFileLines);
        }
    }
}
