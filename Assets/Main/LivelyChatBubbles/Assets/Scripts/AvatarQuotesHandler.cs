using UnityEngine;

namespace LivelyChatBubbles
{
	[RequireComponent(typeof(ChatMouthpiece))]
	public class AvatarQuotesHandler : MonoBehaviour
	{
		[Tooltip("Avatar quote asset to pull random quotes from.")]
		public AvatarQuotes Quotes;

		private ChatMouthpiece _chatMouthpiece;
		private ChatMouthpiece chatMouthpiece { get { if (!_chatMouthpiece) _chatMouthpiece = GetComponent<ChatMouthpiece>(); return _chatMouthpiece; } }

		private void OnMouseUpAsButton()
		{
			Execute();			
		}

		public void Execute()
		{
			if (Quotes && Quotes.Values.Count > 0 && !chatMouthpiece.isSpeaking)
				chatMouthpiece.Speak(Quotes.RandomValue);
		}
	}
}