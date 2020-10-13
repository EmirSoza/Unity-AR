using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LivelyChatBubbles
{
	[Serializable]
	public class ChatOutputProfile : ScriptableObject
	{
		[Tooltip("Time in seconds to wait before starting the chat message.")]
		public float InitialDelay = 0.2f;

		[Tooltip("How many characters per second are output to the chat bubble.")]
		public int CharactersPerSecond = 50;

		[Tooltip("True if the completion requires a mouse click or input key.")]
		public bool CompletionClick = true;

		[Tooltip("Time in seconds to wait after the completion of a chat message.")]
		public float CompletionDelay;

		[Tooltip("Characters that will force a delay time configured by the LongCharacterDelay.")]
		public char[] CharactersForLongDelay;

		[Tooltip("Time in seconds to wait after every outputted character configured by the CharactersForLongDelay.")]
		public float LongCharacterDelay = 0.4f;

		[Tooltip("Characters that will force a delay time configured by the ShortCharacterDelay.")]
		public char[] CharactersForShortDelay;

		[Tooltip("Time in seconds to wait after every outputted character configured by the CharactersForShortDelay.")]
		public float ShortCharacterDelay = 0.2f;
	}

	#if UNITY_EDITOR
	public static class CreateChatOutputProfile
	{
		[MenuItem("Assets/Create/Chat/Output Profile")]
		public static void CreateAsset()
		{
			AssetUtility.CreateAsset<ChatOutputProfile>();
		}
	}
	#endif
}
