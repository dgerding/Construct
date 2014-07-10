using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pretend
{
	class Benchmark
	{
		public static void Begin(String name)
		{
			if (!m_RunTimes.ContainsKey(name))
				m_RunTimes.Add(name, new LinkedList<double>());

			m_LastStartTime = DateTime.Now;
			m_TargetList = m_RunTimes[name];
		}

		public static void End()
		{
			DateTime endTime = DateTime.Now;

			double runTimeMs = (endTime - m_LastStartTime).TotalMilliseconds;
			m_TargetList.AddLast(runTimeMs);
		}

		private static DateTime m_LastStartTime;
		private static LinkedList<double> m_TargetList;

		private static Dictionary<String, LinkedList<double>> m_RunTimes = new Dictionary<string, LinkedList<double>>();
		public static Dictionary<String, LinkedList<double>> RunTimes
		{
			get { return m_RunTimes; }
		}

		public static Dictionary<String, double> AverageTimes
		{
			get
			{
				Dictionary<String, double> result = new Dictionary<string, double>();

				foreach (KeyValuePair<String, LinkedList<double>> benchmark in m_RunTimes)
				{
					result.Add(benchmark.Key, benchmark.Value.Average());
				}

				return result;
			}
		}
	}
}
