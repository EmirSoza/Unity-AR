using UnityEngine;

namespace LivelyChatBubbles
{
	public class Oscillator : MonoBehaviour
	{
		public Vector3 Radius = Vector3.zero;
		public float Speed = 1.0f;
		public bool RandomStartup = false;

		Vector3 Origin;
		float TimeOffset;

		// Use this for initialization
		void Start()
		{
			if (RandomStartup)
				TimeOffset = Random.Range(0, Mathf.PI);
			Origin = transform.localPosition;
		}

		// Update is called once per frame
		void Update()
		{
			transform.localPosition = Origin + Mathf.Sin(Mathf.PI * 2 * (TimeOffset + Time.time) * Speed) * Radius;
		}
	}
}
