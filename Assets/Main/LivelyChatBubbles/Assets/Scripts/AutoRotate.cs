using UnityEngine;

namespace LivelyChatBubbles
{
	public class AutoRotate : MonoBehaviour
	{
		public Vector3 Angle;

		// Update is called once per frame
		void Update()
		{
			transform.Rotate(Angle * Time.deltaTime);
		}
	}
}
