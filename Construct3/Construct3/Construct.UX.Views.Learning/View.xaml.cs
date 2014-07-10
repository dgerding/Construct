using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Construct.UX.ViewModels;
using Telerik.Windows.Controls;


namespace Construct.UX.Views.Learning
{
    public partial class View : Views.View
    {
        public View(ApplicationSessionInfo theApplicationSessionInfo)
        {
            StyleManager.ApplicationTheme = new ModernTheme();

            InitializeComponent();

            InitializeViewModel(theApplicationSessionInfo);
        }

        private ViewModels.Learning.ViewModel SourcesViewModel
        {
            get
            {
                return ViewModel as ViewModels.Learning.ViewModel;
            }
        }

        public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
        {
            if (ViewModel == null)
            {
                ViewModel = new ViewModels.Learning.ViewModel(theApplicationSessionInfo);
            }
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                
            }
        }
    }
}