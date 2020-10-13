using System.Linq;
using UnityEngine;

namespace LivelyChatBubbles
{
	public class ChatZOrder : MonoBehaviour
	{
		void OnEnable()
		{
			InvokeRepeating("Process", 0, 0.25f);
		}

		void OnDisable()
		{
			CancelInvoke("Process");
		}

		void Process()
		{
			// reorder all attached chat bubbles based on distance to the camera
			var camera = Camera.main;
			GetComponentsInChildren<ChatAnchor>()
				.Where(d => d.AttachedBubble)
				.OrderBy(d => Vector3.Distance(d.transform.position, camera.transform.position))
				.Select(d => d.AttachedBubble.rectTransform)
				.ToList()
				.ForEach(d => d.SetAsFirstSibling());
		}
	}
}
