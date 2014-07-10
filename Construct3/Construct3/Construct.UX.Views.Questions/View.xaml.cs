using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using Construct.UX.ViewModels;
using Construct.UX.ViewModels.Questions.QuestionsServiceReference;

namespace Construct.UX.Views.Questions
{
    public partial class View : Views.View
    {
        private string lastExpressionText;
        private Question currentQuestion = new Question();

        private DataType selectedDataType;
        private PropertyType selectedPropertyType;

        public View(ApplicationSessionInfo theApplicationSessionInfo)
        {
            InitializeComponent();

            InitializeViewModel(theApplicationSessionInfo);
        }

        private ViewModels.Questions.ViewModel QuestionsViewModel
        {
            get
            {
                return ViewModel as ViewModels.Questions.ViewModel;
            }
        }

        public void InitializeViewModel(ApplicationSessionInfo theApplicationSessionInfo)
        {
            if (ViewModel == null)
            {
                ViewModel = new ViewModels.Questions.ViewModel(theApplicationSessionInfo);
            }
            if (this.ViewModel.SessionInfo.IsAuthenticated == false)
            {
                Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LayoutRoot.DataContext = QuestionsViewModel;
            }
        }

        private void OnExpressionButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ExpressionEditorWindow window = new ExpressionEditorWindow();
            window.ExpressionEditor.Item = this.QuestionsGridView.CurrentItem;
            window.ExpressionEditor.ExpressionText = this.lastExpressionText;
            
            window.Closed += this.OnExpressionWindowClosed;

            window.ShowDialog();
        }

        private void OnExpressionWindowClosed(object sender, WindowClosedEventArgs e)
        {
            ExpressionEditorWindow window = (ExpressionEditorWindow)sender;
            
            if (window.DialogResult == true)
            {
                GridViewExpressionColumn column = (GridViewExpressionColumn)this.QuestionsGridView.Columns["ExpressionColumn"];

                if (column.Expression != window.ExpressionEditor.Expression)
                {
                    column.ClearFilters();
                    column.Expression = window.ExpressionEditor.Expression;
                }
                this.lastExpressionText = window.ExpressionEditor.ExpressionText;
                currentQuestion.LinqExpression = window.ExpressionEditor.ExpressionText;
            }
        }

        private void DataTypesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                foreach (DataType dataType in e.AddedItems)
                {
                    selectedDataType = dataType;
                    QuestionsViewModel.PropertyTypes = new ObservableCollection<PropertyType>(QuestionsViewModel.GetPropertyTypes().Where(property => property.ParentDataTypeID == dataType.ID));
                    PropertiesComboBox.ItemsSource = QuestionsViewModel.PropertyTypes;
                }
            }
        }

        private void PropertiesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                foreach (PropertyType propType in e.AddedItems)
                {
                    selectedPropertyType = propType;
                    DataType propertyDataType = QuestionsViewModel.DataTypes.Where(dataType => dataType.ID == propType.PropertyDataTypeID).Single();

                    string typeName = propertyDataType.Name;
                    switch (typeName)
                    {
                        case "int":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetIntPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.IntPropertyValues;
                            break;
                        case "bool":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetBoolPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.BoolPropertyValues;
                            break;
                        case "string":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetStringPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.StringPropertyValues;
                            break;
                        case "double":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetDoublePropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.DoublePropertyValues;
                            break;
                        case "float":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetFloatPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.FloatPropertyValues;
                            break;
                        case "Guid":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetGuidPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.GuidPropertyValues;
                            break;
                        case "byte[]":
                            QuestionsViewModel.ClearPropertyValues();
                            QuestionsViewModel.GetByteArrayPropertyValues(selectedDataType.Name, selectedPropertyType.Name);
                            QuestionsGridView.ItemsSource = QuestionsViewModel.ByteArrayPropertyValues;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public class PropertyValue
    {
        private static Random rand = new Random(new DateTime().Millisecond);
        public PropertyValue()
        {
            StartTime = DateTime.Now;
            Interval = rand.Next(1, 1000000);
            Latitude = rand.Next(-100, 100).ToString();
            Longitude = rand.Next(-100, 100).ToString();
        }

        public DateTime StartTime { get; set; }
        public long Interval { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}