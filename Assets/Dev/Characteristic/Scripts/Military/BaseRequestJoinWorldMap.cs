using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseRequestJoinWorldMap 
{
	public long Uid;
	public string ChannelId;
	public void SetData(BaseRequestJoinWorldMap data)
	{
		Uid = data.Uid;
		ChannelId = data.ChannelId;
	}
}
