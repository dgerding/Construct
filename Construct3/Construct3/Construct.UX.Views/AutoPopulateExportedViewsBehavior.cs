using System;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;

namespace Construct.UX.Views
{
    [Export(typeof(AutoPopulateExportedViewsBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AutoPopulateExportedViewsBehavior : RegionBehavior, IPartImportsSatisfiedNotification
    {
        [ImportMany(AllowRecomposition = true)]
        public Lazy<object, IViewRegionRegistration>[] RegisteredViews { get; set; }

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            AddRegisteredViews();
        }

        #endregion

        protected override void OnAttach()
        {
            AddRegisteredViews();
        }

        private void AddRegisteredViews()
        {
            if (this.Region != null)
            {
                foreach (var viewEntry in RegisteredViews)
                {
                    if (viewEntry.Metadata.RegionName == Region.Name)
                    {
                        object view = viewEntry.Value;

                        if (!this.Region.Views.Contains(view))
                        {
                            Region.Add(view);
                        }
                    }
                }
            }
        }
    }
}
