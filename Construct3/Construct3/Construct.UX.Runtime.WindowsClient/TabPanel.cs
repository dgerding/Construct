using System;
using System.Linq;
using System.Windows.Controls;

namespace Construct.UX.WindowsClient
{
    public class TabPanel
    {
        private UserControl regionControl;

        private string regionName;

        private string fullRegionName;

        private string tabName;

        private string subTabName;

        private string ribbonBarName;

        private int tabOrder;

        private int subTabOrder;

        public TabPanel(UserControl anItemControl, string aFullRegionName, string aRegionName, string aTabName, int aTabOrder, int aSubTabOrder, string aRibbonBarName)
        {
            regionControl = anItemControl;
            fullRegionName = aFullRegionName;
            regionName = aRegionName;
            tabName = aTabName;
            tabOrder = aTabOrder;
            subTabOrder = aSubTabOrder;
            ribbonBarName = aRibbonBarName;
            // null, regionName, tabName, tabOrder, subTabOrder, ribbonBarName
        }

        public UserControl RegionControl
        {
            get
            {
                return regionControl;
            }
            set
            {
                regionControl = value;
            }
        }

        public string RegionName
        {
            get
            {
                return regionName;
            }
        }

        public string FullRegionName
        {
            get
            {
                return fullRegionName;
            }
        }

        public string TabName
        {
            get
            {
                return tabName;
            }
        }

        public string SubTabName
        {
            get
            {
                return subTabName;
            }
        }

        public string RibbonBarName
        {
            get
            {
                return ribbonBarName;
            }
        }

        public int TabOrder
        {
            get
            {
                return tabOrder;
            }
        }

        public int SubTabOrder
        {
            get
            {
                return subTabOrder;
            }
        }
    }
}