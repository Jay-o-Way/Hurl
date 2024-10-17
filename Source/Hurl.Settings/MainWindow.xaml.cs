using Hurl.Settings.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Windows.Graphics;
using Windows.UI.Xaml.Navigation;

namespace Hurl.Settings;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;

        AppWindow.SetIcon("internet.ico");
        //SystemBackdrop = new MicaBackdrop();
        SystemBackdrop = new DesktopAcrylicBackdrop();
        AppWindow.ResizeClient(new SizeInt32(700, 500));
        // TO-DO: set minimum size
    }


    // Examples copied from https://learn.microsoft.com/windows/apps/design/controls/navigationview

    private double NavViewCompactModeThresholdWidth { get { return NavView.CompactModeThresholdWidth; } }

    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Add handler for ContentFrame navigation.
        ContentFrame.Navigated += On_Navigated;

        // NavView doesn't load any page by default, so load home page.
        NavView.SelectedItem = NavView.MenuItems[0];

        // If navigation occurs on SelectionChanged, this isn't needed.
        // Because we use ItemInvoked to navigate, we need to call Navigate
        // here to load the home page.
        var cmdArgs = Environment.GetCommandLineArgs();
        if (cmdArgs.Length > 1)
        {
            Debug.WriteLine(cmdArgs[0]);
            var primaryArg = cmdArgs[1];

            if (primaryArg.Equals("--page"))
            {
                if (cmdArgs.Length > 2)
                {
                    //m_window.NavigateToPage(args[2]);
                    //NavView_Navigate(cmdArgs[2], new EntranceNavigationTransitionInfo());
                    // TO-DO: fix conversion from string to type
                }
            }
        }
        else
        {
            NavView_Navigate(typeof(BrowsersPage), new EntranceNavigationTransitionInfo());
        }
        Activate();
    }

    private void On_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NavView_ItemInvoked(NavigationView sender,
                                     NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked == true)
        {
            NavView_Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        }
        else if (args.InvokedItemContainer != null)
        {
            Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
            NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
        }
    }

    // NavView_SelectionChanged is not used in this example, but is shown for completeness.
    // You will typically handle either ItemInvoked or SelectionChanged to perform navigation,
    // but not both.
    private void NavView_SelectionChanged(NavigationView sender,
                                          NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected == true)
        {
            NavView_Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        }
        else if (args.SelectedItemContainer != null)
        {
            Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
            NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
        }
    }

    private void NavView_Navigate(
        Type navPageType,
        NavigationTransitionInfo transitionInfo)
    {
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack.
        Type preNavPageType = ContentFrame.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded.
        if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
        {
            ContentFrame.Navigate(navPageType, null, transitionInfo);
        }
    }

    private void NavView_BackRequested(NavigationView sender,
                                       NavigationViewBackRequestedEventArgs args)
    {
        TryGoBack();
    }

    private bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
            return false;

        // Don't go back if the nav pane is overlayed.
        if (NavView.IsPaneOpen &&
            (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
             NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            return false;

        ContentFrame.GoBack();
        return true;
    }

    private void On_Navigated(object sender, NavigationEventArgs e)
    {
        NavView.IsBackEnabled = ContentFrame.CanGoBack;

        if (ContentFrame.SourcePageType == typeof(SettingsBase))
        {
            // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
            NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            NavView.Header = "Settings";
        }
        else if (ContentFrame.SourcePageType != null)
        {
            // Select the nav view item that corresponds to the page being navigated to.
            NavView.SelectedItem = NavView.MenuItems
                        .OfType<NavigationViewItem>()
                        .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));

            NavView.Header =
                ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();

        }
    }
}
