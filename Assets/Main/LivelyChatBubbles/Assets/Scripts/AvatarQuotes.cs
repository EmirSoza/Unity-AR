using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LivelyChatBubbles
{
	[Serializable]
	public class AvatarQuotes : ScriptableObject
	{
		public List<string> Values;
		public string RandomValue
		{
			get { return Values[UnityEngine.Random.Range(0, Values.Count - 1)]; }
		}
	}

	#if UNITY_EDITOR
	public static class CreateQuotes
	{
		[MenuItem("Assets/Create/Chat/Avatar Quotes")]
		public static void CreateAsset()
		{
			AssetUtility.CreateAsset<AvatarQuotes>();
		}
	}
	#endif
}
