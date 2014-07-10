using System;
using System.Linq;
using System.Windows.Controls;

namespace Construct.UX.Views
{
    public class View : UserControl
    {
        //TODO: Put the sesssion info
        public View()
        {
        }

        public ViewModels.ViewModel ViewModel { get; set; }
    }
}