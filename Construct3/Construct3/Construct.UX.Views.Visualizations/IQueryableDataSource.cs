﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construct.MessageBrokering.Serialization;

namespace Construct.UX.Views.Visualizations
{
	public interface IQueryableDataSource
	{
		IEnumerable<SimplifiedPropertyValue> GetData(DateTime startTime, DateTime endTime, DataSubscription dataToGet);
		void Connect();
		void Disconnect();
	}
}
