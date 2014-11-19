using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construct.UX.Views.Visualizations
{
	//	Translates Subscription Guids to names (i.e. source name, property name, data type name)
	class SubscriptionTranslator
	{
		private Dictionary<DataSubscription, SubscriptionLabel> subscriptionTranslations = new Dictionary<DataSubscription, SubscriptionLabel>();

		public void AddTranslation(DataSubscription subscription, SubscriptionLabel label)
		{
			subscriptionTranslations.Add(subscription, label);
		}
		public SubscriptionLabel GetTranslation(DataSubscription subscription)
		{
			SubscriptionLabel translation;
			if (subscriptionTranslations.TryGetValue(subscription, out translation))
				return translation;
			else
				return null;
		}

		public Dictionary<DataSubscription, SubscriptionLabel> AllTranslations()
		{
			return subscriptionTranslations;
		}
	}
}
