using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.UX.ViewModels.Meaning
{
    public class CallbackImplementation : MeaningServiceReference.IModelCallback
    {
        #region IModelCallback Members

        public CallbackImplementation()
        {
        }

        public void AddSemanticSubjectCallbackReceived(string theSubject)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginAddSemanticSubjectCallbackReceived(string theSubject, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndAddSemanticSubjectCallbackReceived(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void AddSemanticPredicateCallbackReceived(string thePredicate)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginAddSemanticPredicateCallbackReceived(string thePredicate, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndAddSemanticPredicateCallbackReceived(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void AddSemanticObjectCallbackReceived(string theObject)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginAddSemanticObjectCallbackReceived(string theObject, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndAddSemanticObjectCallbackReceived(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}