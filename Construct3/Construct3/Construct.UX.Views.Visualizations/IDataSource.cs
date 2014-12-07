using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	public interface IDataSource
	{
		event Action<SimplifiedPropertyValue> OnData;

		bool IsQueryable { get; }

		IEnumerable<SimplifiedPropertyValue> GetData(DateTime startTime, DateTime endTime, DataSubscription dataToGet); 

		void AddSubscription(Guid sourceId, Guid propertyId);
		void RemoveSubscription(Guid sourceId, Guid propertyId);
		void Connect();
		void Disconnect();
	}
}
