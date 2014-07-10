using System;
using System.Linq;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Construct.UX.WindowsClient
{
    /// <summary>
    /// Interaction logic for LoginPanel.xaml
    /// </summary>
    public partial class ShellLoginPanel : UserControl
    {
        // Create a custom routed event by first registering a RoutedEventID
        public ShellLoginPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            InitializeComponent();
        }
    }
}