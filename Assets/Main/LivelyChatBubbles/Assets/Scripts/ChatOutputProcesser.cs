using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LivelyChatBubbles
{
	[RequireComponent(typeof(ChatBubble))]
	public class ChatOutputProcesser : MonoBehaviour
	{
		[Tooltip("Complete value to inject over time.")]
		public string Value;

		[Tooltip("Rules for the processor to follow over time.")]
		public ChatOutputProfile Profile;

		[Tooltip("Audio source to play as characters are output.")]
		public AudioSource AudioSource;

		ChatBubble _chatBubble;
		ChatBubble chatBubble { get { if (!_chatBubble) _chatBubble = GetComponent<ChatBubble>(); return _chatBubble; } }

		Vector2 previousSize;

		float charTime;
		int charsInFrame;
		int charIndex;
		char[] charTemplate;

		private void OnEnable()
		{
			if (Value == null || Profile == null)
				return;

			charTime = 0;
			charsInFrame = 0;
			charIndex = 0;
			chatBubble.BindMessageValue(string.Empty);
			previousSize = chatBubble.PerformManualSize(Value);
			charTemplate = GetValueTemplate(Value);

			StartCoroutine(Process());
		}

		private void OnDisable()
		{
			if (Value == null || Profile == null)
				return;

			StopCoroutine(Process());

			chatBubble.MessageMinimumSize = previousSize;
		}

		IEnumerator Process()
		{
			// initial delay
			if (Profile.InitialDelay > 0)
				yield return new WaitForSeconds(Profile.InitialDelay);

			// locals
			var lastProcessedTime = Time.time;
			var thisProcessedTime = lastProcessedTime;
			var processedAtLeastOneNonSpace = false;
			var pauseTime = 0F;

			// loop until the entire value has been outputted
			while ( charIndex < Value.Length )
			{
				// skip a frame
				yield return null;

				thisProcessedTime = Time.time;

				// buildup chars until we have at least a whole value
				charTime += (thisProcessedTime - lastProcessedTime) * (float)Profile.CharactersPerSecond;

				lastProcessedTime = thisProcessedTime;

				// no need to process if there isn't a chunk ready
				if (charTime < 1)
					continue;

				// take whole value or whatever is left in our value 
				charsInFrame = Mathf.Min((int)charTime, Value.Length - charIndex);

				// chop off the whole value from our buildup
				charTime %= charsInFrame;

				// compute the next chunck to append
				processedAtLeastOneNonSpace = false;
				while (charsInFrame > 0 && charIndex < Value.Length)
				{
					if (charTemplate[charIndex] == ' ')
					{
						charTemplate[charIndex] = Value[charIndex];
						if (charTemplate[charIndex] != ' ')
							processedAtLeastOneNonSpace = true;

						// add some delays per char configurations 
						foreach (var delayCharInfo in Profile.CharactersForLongDelay)
							if (charTemplate[charIndex] == delayCharInfo)
								pauseTime += Profile.LongCharacterDelay;
						foreach (var delayCharInfo in Profile.CharactersForShortDelay)
							if (charTemplate[charIndex] == delayCharInfo)
								pauseTime += Profile.ShortCharacterDelay;

						charsInFrame--;
					}
					charIndex++;
				}

				// append the chunk
				chatBubble.BindMessageValue(new string(charTemplate));

				// play audio
				if (processedAtLeastOneNonSpace && AudioSource && AudioSource.clip)
					AudioSource.Play();

				// wait here for a bit if we ran into any pause characters
				if ( pauseTime > 0 )
				{
					yield return new WaitForSeconds(pauseTime);
					lastProcessedTime = Time.time;
					pauseTime = 0;
				}
			}

			// final wait for click or delay
			if (Profile.CompletionClick)
			{
				// wait for click
				while (!Input.GetMouseButtonUp(0) && !Input.GetKeyUp(KeyCode.Return) )
					yield return null;
			}
			else
			{
				// delay
				yield return new WaitForSeconds(Profile.CompletionDelay);
			}

			// disable
			enabled = false;
		}

		// text <b>bold here</b> <color="#FFFFFF">color here <b>bold with color</b></color> and a lower font size <size=8>large size with some <i>italics here</i></size>!!
		//      <b>         </b> <color="#FFFFFF">           <b>               </b></color>                       <size=8>                     <i>            </i></size>  
		char[] GetValueTemplate( string value )
		{
			var matches = Regex.Matches(value, @"<[^>]*>").Cast<Match>().ToArray();
			var template = new char[value.Length];
			for (int c = 0; c < value.Length; c++)
				template[c] = ' ';
			for (int m = 0; m < matches.Length; m++)
				for (int c = 0; c < matches[m].Value.Length; c++)
					template[matches[m].Index + c] = matches[m].Value[c];
			return template;			
		}
	}

}
