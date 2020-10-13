using UnityEngine;

namespace LivelyChatBubbles
{
	public class AutoFace : MonoBehaviour
	{
		public Transform Target;

		void Update()
		{
			if (Target)
				transform.LookAt(Target);
		}
	}
}
