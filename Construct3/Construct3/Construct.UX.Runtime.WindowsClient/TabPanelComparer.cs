using System;
using System.Collections.Generic;
using System.Linq;

namespace Construct.UX.WindowsClient
{
    class TabPanelComparer : IComparer<TabPanel>
    {
        public int Compare(TabPanel x, TabPanel y)
        {
            if (x.TabOrder < y.TabOrder)
            {
                return -1;
            }

            if (x.TabOrder == y.TabOrder)
            {
                return 0;
            }

            if (x.TabOrder > y.TabOrder)
            {
                return 1;
            }

            return 1;
        }
    }
}