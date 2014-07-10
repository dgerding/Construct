using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.UX.ViewModels.Data
{
    public class CallbackImplementation : DataServiceReference.IModelCallback
    {
        public void HandleItem(DataServiceReference.Datum datum)
        {
            // TODO: Implement this method
            //throw new NotImplementedException();
        }

        public IAsyncResult BeginHandleItem(DataServiceReference.Datum datum, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException("Async contract not implemented, dude.");
        }

        public void EndHandleItem(IAsyncResult result)
        {
        }
    }
}
