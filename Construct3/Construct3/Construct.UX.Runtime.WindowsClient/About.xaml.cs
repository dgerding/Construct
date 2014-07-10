using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Construct.UX.WindowsClient
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: Update Runtime and app stats here
        }
    }
}