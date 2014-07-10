using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace Construct.Server.Models.Data
{
    [CollectionDataContract]
    public class LiveCollection<T> : ICollection<T>
        where T : class
    {
        private List<T> currentItems = null;
        private OpenAccessContext context = null;
        private MetadataSource source = null;

        public LiveCollection(string connectionString)
        {
            Type collectionType = typeof(T);
            IEnumerable<MetadataSource> sources = FluentMetadataSource.FromAssembly(typeof(Entities.EntitiesModel).Assembly);
            foreach (MetadataSource target in sources)
            {
                if (source == null)
                {
                    source = target;
                }
                else
                {
                    source = new AggregateMetadataSource(source.GetModel(), target.GetModel());
                }
            }
            context.Events.Added += new AddEventHandler(Events_Added);
            context.Events.Removed += new RemoveEventHandler(Events_Removed);
        }

        void Events_Removed(object sender, RemoveEventArgs e)
        {
            T item = e.PersistentObject as T;
            currentItems.Remove(item);
        }

        void Events_Added(object sender, AddEventArgs e)
        {
            T item = e.PersistentObject as T;
            currentItems.Add(item);
        }


        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return currentItems.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return currentItems.GetEnumerator();
        }
        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            context.Add(item);
        }

        public void Clear()
        {
            context.ClearChanges();
        }

        public bool Contains(T item)
        {
            return currentItems.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            currentItems.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return currentItems.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            context.Delete(item);
            return true;
        }

        #endregion
    }
}