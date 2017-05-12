﻿namespace EveCentralProvider.Types
{
	public class AggregateInfoList
	{
		public AggregateInfo[] Items { get; set; }
	}

	public class AggregateInfo
	{
		public string id { get; set; }

		public AggreateInfoStat buy { get; set; }

		public AggreateInfoStat sell { get; set; }
	}

	public class AggreateInfoStat
	{
		public double weightedAverage { get; set; }

		public float max { get; set; }

		public float min { get; set; }

		public double stddev { get; set; }

		public float median { get; set; }

		public float volume { get; set; }

		public int orderCount { get; set; }

		public float percentile { get; set; }
	}
}
