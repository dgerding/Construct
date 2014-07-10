using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;

namespace Construct.UX.ViewModels.Sources
{
    public class CallbackImplementation : SourcesServiceReference.IModelCallback
    {
        #region IModelCallback Members
        public event Action<Guid> OnSensorLoadedCallbackReceived;
        public event Action<Guid> OnSensorInstalledCallbackReceived;
        public event Action<List<SourcesServiceReference.SensorCommand>> OnAvailableSensorCommandsCallbackReceived;

        public CallbackImplementation()
        {
        }

        public void SensorLoadedCallbackReceived(Guid guid)
        {
            if (OnSensorLoadedCallbackReceived != null)
            {
                OnSensorLoadedCallbackReceived(guid);
            }
        }

        public void SensorLoadedCallbackReceivedAsync(Guid guid)
        {
            BeginSensorLoadedCallbackReceived(guid, EndSensorLoadedCallbackReceived, null);
        }

        public IAsyncResult BeginSensorLoadedCallbackReceived(Guid guid, AsyncCallback callback, object asyncState)
        {
            Action<Guid> method = SensorLoadedCallbackReceived;
            IAsyncResult result = method.BeginInvoke(guid, callback, asyncState);
            return result;
        }

        public void EndSensorLoadedCallbackReceived(IAsyncResult result)
        {
        }

        public void SensorInstalledCallbackReceived(Guid guid)
        {
            if (OnSensorInstalledCallbackReceived != null)
            {
                OnSensorInstalledCallbackReceived(guid);
            }
        }

        public void SensorInstalledCallbackReceivedAsync(Guid guid)
        {
            Action<IAsyncResult> callback = (target) => EndSensorInstalledCallbackReceived(target);
            AsyncCallback result = new AsyncCallback(callback);
            BeginSensorLoadedCallbackReceived(guid, result, null);
        }

        public IAsyncResult BeginSensorInstalledCallbackReceived(Guid guid, AsyncCallback callback, object asyncState)
        {
            Action<Guid> method = SensorInstalledCallbackReceived;
            IAsyncResult result = method.BeginInvoke(guid, callback, asyncState);
            return result;
        }

        public void EndSensorInstalledCallbackReceived(IAsyncResult result)
        {
        }

        public void AvailableSensorCommandsCallbackReceived(List<SourcesServiceReference.SensorCommand> commands)
        {
            if (OnAvailableSensorCommandsCallbackReceived != null)
            {
                OnAvailableSensorCommandsCallbackReceived(commands);
            }
        }


        #endregion
    }
}