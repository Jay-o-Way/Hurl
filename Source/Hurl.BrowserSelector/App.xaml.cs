﻿using Hurl.BrowserSelector.Globals;
using Hurl.BrowserSelector.Helpers;
using Hurl.BrowserSelector.Views;
using Hurl.BrowserSelector.Views.ViewModels;
using SingleInstanceCore;
using System.Windows;

namespace Hurl.BrowserSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        private MainWindow _mainWindow;
        private MainViewModel viewModel;
        //private readonly Settings settings = SettingsFile.GetSettings();

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isFirstInstance = this.InitializeAsFirstInstance("HurlTray");
            if (isFirstInstance)
            {
                var x = CliArgs.GatherInfo(e.Args, false);

                CurrentLink.Value = x.Url;
                viewModel = new MainViewModel();

                _mainWindow = new MainWindow(viewModel.settings)
                {
                    DataContext = viewModel,
                };

                _mainWindow.Init(x);
            }
            else
            {
                Current.Shutdown();
            }
        }

        public void OnInstanceInvoked(string[] args)
        {
            Current.Dispatcher.Invoke(() =>
            {
                var x = CliArgs.GatherInfo(args, true);
                var IsTimedSet = TimedBrowserSelect.CheckAndLaunch(x.Url);

                if (!IsTimedSet)
                {
                    CurrentLink.Value = x.Url;
                    _mainWindow.Init(x);
                }

            });
        }

        protected override void OnExit(ExitEventArgs e) => SingleInstance.Cleanup();
    }
}