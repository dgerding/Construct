using System;
using System.Linq;
using System.ComponentModel.Composition;

namespace Construct.UX.Views
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public class ViewExportAttribute : ExportAttribute, IViewRegionRegistration
    {
        public ViewExportAttribute()
            : base(typeof(object))
        {
        }

        public ViewExportAttribute(string viewName)
            : base(viewName, typeof(object))
        {
        }

        #region IViewRegionRegistration Members

        public string RegionName { get; set; }
        #endregion
    }

}
