using System;
using System.Linq;
using Construct.Server.Entities;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace Construct.Server.Models.Sources
{
    public class SensorInstaller
    {
        string connectionString = null;
        Data.Model dataModel = null;

        public SensorInstaller(string connectionString, Data.Model dataModel)
        {
            this.connectionString = connectionString;
            this.dataModel = dataModel;
        }

        public bool AddSensorEntities(SensorRuntime sensorRuntime)
        {
            if (dataModel.AddType(sensorRuntime.InstallerXml))
            {
                AddSensorRuntime(sensorRuntime);
                return true;
            }
            return false;
        }

        private void AddSensorRuntime(SensorRuntime sensorRuntime)
        {
            EntitiesModel context = new EntitiesModel(connectionString);
            context.Add(sensorRuntime);

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new ApplicationException("AddSensorRuntimeToEntities failed" + " " + e.Message);
            }
        }
    }
}
