using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using Construct.UX.ViewModels.Meaning;
using Construct.UX.ViewModels.Meaning.MeaningServiceReference;

namespace Construct.UX.Views.Meaning
{
    /// <summary>
    /// Interaction logic for SensorHostsPanel.xaml
    /// </summary>
    public partial class TaxonomyPanel : UserControl
    {
        public ViewModel TheViewModel;

        public TaxonomyPanel()
        {
            StyleManager.ApplicationTheme = new ModernTheme();
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //can we put this in a constructor?
        }

        private void AddTaxonomy_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(AddTaxonomyTextBox.Text) == false)
            {
                dynamic parameter = new ExpandoObject();
                parameter.ID = Guid.NewGuid();
                parameter.Name = AddTaxonomyTextBox.Text;
                TheViewModel.AddTaxonomy(parameter);
            }
        }

        private void AddLabel_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(AddLabelTextBox.Text) == false)
            {
                dynamic parameter = new ExpandoObject();
                parameter.ID = Guid.NewGuid();
                parameter.Name = AddLabelTextBox.Text;
                TheViewModel.AddLabel(parameter);
            }
        }

        private void LinkTaxonomyLabel_Click(object sender, RoutedEventArgs e)
        {
            Construct.UX.ViewModels.Meaning.MeaningServiceReference.Taxonomy selectedTaxonomy = (Construct.UX.ViewModels.Meaning.MeaningServiceReference.Taxonomy)TaxonomyComboBox.SelectedValue;
            Construct.UX.ViewModels.Meaning.MeaningServiceReference.Label selectedLabel = (Construct.UX.ViewModels.Meaning.MeaningServiceReference.Label)LabelComboBox.SelectedValue;

            if (selectedTaxonomy != null && selectedLabel != null)
            {
                if (String.IsNullOrEmpty(selectedTaxonomy.Name) == false &&
                    String.IsNullOrEmpty(selectedLabel.Name) == false)
                {
                    dynamic parameter = new ExpandoObject();
                    parameter.ID = Guid.NewGuid();
                    parameter.TaxonomyID = selectedTaxonomy.ID;
                    parameter.LabelID = selectedLabel.ID;
                    TheViewModel.AddTaxonomyLabel(parameter);
                }
            }
        }
    }
}