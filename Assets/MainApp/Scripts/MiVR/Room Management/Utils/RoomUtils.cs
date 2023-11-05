using System;
using LucidSightTools;
using Newtonsoft.Json;
using UnityEngine;

namespace jp.co.mirabo.Application.RoomManagement
{
	public class RoomUtils
	{
		public static T OnReceivedDataAndParse<T>(string data)
		{
			LSLog.Log($"Parse Data: {data}");

			try
			{
				return JsonConvert.DeserializeObject<T>(data);
			}
			catch (Exception ex)
			{
				DebugExtension.LogError($"Error: {ex.Message}");
				return default(T);
			}
		}
	}
}