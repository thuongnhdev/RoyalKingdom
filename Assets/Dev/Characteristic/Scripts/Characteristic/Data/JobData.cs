using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
	[System.Serializable]
	public class JobData
	{
		public int Key;
		public string JobName;
		public int JobGrade;
		public int JobClass;
		public int Level;
		public List<BaseStatsOfJob> StatsData;
		public void SetData(JobData data)
		{
			Key = data.Key;
			JobName = data.JobName;
			JobClass = data.JobClass;
			JobGrade = data.JobGrade;
			Level = data.Level;
			StatsData = data.StatsData;
		}
	}

	[System.Serializable]
	public class BaseStatsOfJob
	{
		public int Key;
		public string StatName;
		public int Value;
		public void SetData(BaseStatsOfJob data)
		{
			Key = data.Key;
			StatName = data.StatName;
			Value = data.Value;
		}
	}
}