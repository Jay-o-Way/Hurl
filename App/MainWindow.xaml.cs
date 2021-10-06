﻿using Hurl.Controls;
using Hurl.Services;
using Hurl.Services.AppSettings;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hurl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string OpenedLink = null;

        public MainWindow(string[] x)
        {
            InitializeComponent();

            Window_Loaded();

            if (x.Length >= 1)
            {
                string link = x[0];
                if (link.StartsWith("hurl://"))
                {
                    OpenedLink = linkpreview.Text = link.Substring(7);
                }
                else
                {
                    OpenedLink = linkpreview.Text = x[0];
                }
            }

            //What does \"%1\" mean in Registry ? 
            //https://www.tek-tips.com/viewthread.cfm?qid=382878
            //Environment.GetCommandLineArgs()[0] + 
        }

        private void Window_Esc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e) => Close();


        private void Window_Loaded()
        {
            BrowsersList x = new SettingsFile().ReadSettingsFile().Browsers;

            foreach (BrowserObject i in x)
            {
                if (i.Name != null)
                {
                    BrowserIconBtn browserUC = new BrowserIconBtn()
                    {
                        BrowserName = i.Name,
                        BrowserIcon = i.GetIcon,
                        ExePath = i.ExePath,
                        URL = OpenedLink
                    };

                    var separator = new System.Windows.Shapes.Rectangle()
                    {
                        Width = 1,
                        Height = 40,
                        Margin = new Thickness(3, 0, 3, 20),
                        Fill = System.Windows.Media.Brushes.AliceBlue
                    };

                    _ = stacky.Children.Add(browserUC);
                    _ = stacky.Children.Add(separator);
                }

            }

            stacky.Children.RemoveAt(stacky.Children.Count - 1);
        }

        // TODO
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void LinkCopyBtn(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OpenedLink);
        }
    }
}
