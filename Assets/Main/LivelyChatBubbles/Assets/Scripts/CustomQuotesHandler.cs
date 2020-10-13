using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LivelyChatBubbles
{
	public class CustomQuotesHandler : MonoBehaviour
	{
		[Tooltip("List of mouthpieces to choose from upon execution.")]
		public List<ChatMouthpiece> Mouthpieces;
		
		public void Execute( string message )
		{
			var randomIndices = Enumerable.Range(0, Mouthpieces.Count).OrderBy(d => Random.value).ToArray();
			foreach( var ri in randomIndices )
				if (!Mouthpieces[ri].isSpeaking)
				{
					Mouthpieces[ri].Speak(message);
					break;
				}
		}
	}
}