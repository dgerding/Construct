using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ScrubbeR
{
	public class TestHub : Hub
	{
		public void TimeChange(String newTime)
		{
			Clients.All.TimeChange(newTime);
		}
	}
}