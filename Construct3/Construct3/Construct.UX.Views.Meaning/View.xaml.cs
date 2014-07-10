﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using Construct.UX.ViewModels;

namespace Construct.UX.Views.Meaning
{
    public partial class View : Views.View
    {
        public View(ApplicationSessionInfo theApplicationSessionInfo)
        {
            StyleManager.ApplicationTheme = new ModernTheme();

            InitializeComponent();

            InitializeViewModel(theApplicationSessionInfo);
        }

        private ViewModels.Meaning.ViewModel MeaningViewModel
        {
            get
            {
                return ViewModel as ViewModels.Meaning.ViewModel;
            }
        }

        public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
        {
            if (ViewModel == null)
            {
                ViewModel = new ViewModels.Meaning.ViewModel(theApplicationSessionInfo);
            }
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LayoutRoot.DataContext = MeaningViewModel;

                theTaxonomyPanel.TheViewModel = MeaningViewModel;
                theTaxonomyPanel.DataContext = MeaningViewModel;
            }
        }
    }
}