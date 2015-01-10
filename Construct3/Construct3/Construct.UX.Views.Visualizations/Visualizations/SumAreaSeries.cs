using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.ChartView;

namespace Construct.UX.Views.Visualizations.Visualizations
{
	//	Used for the TextPropertyPreview, which shows an AreaSeries with
	//		the quantity of text values that were received over a period of time

	class SumAreaSeries : AreaSeries
	{
		protected override ChartAggregateFunction GetValueAggregateFunction()
		{
			return new ChartSumFunction();
		}
	}

	class ChartSumFunction : ChartAggregateFunction
	{
		protected override string AggregateMethodName
		{
			get { return "Sum"; }
		}
	}
}
