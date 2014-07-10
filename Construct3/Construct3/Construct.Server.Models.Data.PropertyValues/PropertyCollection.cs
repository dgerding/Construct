using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Construct.Server.Models.Data.PropertyValue
{
    public class PropertyCollection<P, T> : ObservableCollection<P>
        where P : PropertyValue<T>, new()
    {
    }
}