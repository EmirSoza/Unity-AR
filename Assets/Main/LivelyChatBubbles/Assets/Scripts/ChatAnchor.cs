using UnityEngine;

namespace LivelyChatBubbles
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Transform))]
	public class ChatAnchor : MonoBehaviour
	{
		[Tooltip("Chat Bubble that should be anchored at this transform.")]
		public ChatBubble AttachedBubble;
		public bool BindAttachedBubble( ChatBubble value )
		{
			if (AttachedBubble == value)
				return false;
			AttachedBubble = value;
			snapTracking = true;
			return true;
		}

		[Tooltip("Radius in world units from the anchor transform to the bubble's pivot.")]
		public float AttachedRadius = 1;

		[Range(-180, 180)]
		[Tooltip("Angle in degrees around the transform to the bubble's pivot.")]
		public float AttachedAngle = 90;

		[Range(0, 1)]
		[Tooltip("Percentage to influence the bubble's angle towards the center of the screen.")]
		public float CenterInfluence = 0.0f;

		[Tooltip("Smoothing speed as the bubble follows the anchor transform.")]
		public float TrackingSpeed = 15f;

		[Tooltip("True if the bubble should stay within the screen bounds until the anchor position is no longer visible.")]
		public bool KeepInView = true;
		
		bool snapTracking;
		bool inViewport;

		void OnEnable()
		{
			snapTracking = true;
		}
		
		void OnValidate()
		{
			if (AttachedRadius < 0)
				AttachedRadius = 0;
		}

		void Update()
		{
			if (AttachedBubble)
			{
				if (!Application.isPlaying)
					snapTracking = true;

				var camera = Camera.main;
				var cameraTransform = camera.transform;
				var halfScreenSize = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);

				// check for visibility change
				var ourViewportPosition = camera.WorldToViewportPoint(transform.position);

				// hide if attachment is no longer visible
				if (inViewport && (ourViewportPosition.x < 0 || ourViewportPosition.x > 1 || ourViewportPosition.y < 0 || ourViewportPosition.y > 1 || ourViewportPosition.z < 0))
				{
					inViewport = false;
					AttachedBubble.rectTransform.position = new Vector3(-1000000, -1000000, 0);
				}

				// show if attachment becomes visible
				if (!inViewport && ourViewportPosition.x >= 0 && ourViewportPosition.x <= 1 && ourViewportPosition.y >= 0 && ourViewportPosition.y <= 1 && ourViewportPosition.z >= 0)
				{
					inViewport = true;
					snapTracking = true;
				}

				if (inViewport)
				{
					// auto calculate angle based on screen position -> center 
					var adjustedAngle = AttachedAngle;
					if (CenterInfluence > 0)
					{
						var vectorToCamera = halfScreenSize - camera.WorldToScreenPoint(transform.position);
						var angleToCenter = Mathf.Atan2(vectorToCamera.y, vectorToCamera.x) * Mathf.Rad2Deg;
						adjustedAngle = Mathf.LerpAngle(adjustedAngle, angleToCenter, vectorToCamera.magnitude / Mathf.Min(halfScreenSize.x, halfScreenSize.y) * CenterInfluence);
					}

					// initialize position to attachment + offset of angle/radius
					var adjustedPosition = transform.position + Quaternion.AngleAxis(adjustedAngle, camera.transform.forward) * camera.transform.right * AttachedRadius;

					// convert screen coords
					adjustedPosition = camera.WorldToScreenPoint(adjustedPosition); adjustedPosition.z = AttachedBubble.rectTransform.position.z;

					// adjust for out of bounds
					if (KeepInView)
					{
						adjustedPosition.x = Mathf.Clamp(adjustedPosition.x, AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * AttachedBubble.rectTransform.pivot.x, camera.pixelWidth - AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * (1 - AttachedBubble.rectTransform.pivot.x));
						adjustedPosition.y = Mathf.Clamp(adjustedPosition.y, AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * AttachedBubble.rectTransform.pivot.y, camera.pixelHeight - AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * (1 - AttachedBubble.rectTransform.pivot.y));
					}

                    // fix bug in 2017.4.2f2 where setting pivot would reset position
                    // so now we just set the position AFTER the pivot
                    var position = Vector3.Lerp(AttachedBubble.rectTransform.position, adjustedPosition, snapTracking ? 1.0f : Time.deltaTime * TrackingSpeed);

                    // adjust panels rect pivot to be the opposite side of our screen pivot angle
                    if (AttachedRadius == 0)
                        AttachedBubble.rectTransform.pivot = Vector2.one / 2f;
                    else
                        AttachedBubble.rectTransform.pivot = new Vector2(
                            Mathf.Cos(adjustedAngle * Mathf.Deg2Rad).Remap(-1, 1, 1, 0),
                            Mathf.Sin(adjustedAngle * Mathf.Deg2Rad).Remap(-1, 1, 1, 0));

                    // set it
                    AttachedBubble.rectTransform.SetPositionAndRotation(position, AttachedBubble.rectTransform.rotation);

                    if (snapTracking)
						snapTracking = false;

					// adjust bubble extender dock and position
					if (AttachedBubble.ExtenderComponent)
					{
						if (AttachedRadius == 0)
						{
							AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Bottom);
							AttachedBubble.BindExtenderPosition(0);
						}
						else
						{
							// adjust extender dock based on opposite pivot
							var pivot = AttachedBubble.rectTransform.pivot;
							if (pivot.x <= 0.1464466)
								AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Left);
							else if (pivot.x >= 0.8535534)
								AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Right);
							else if (pivot.y <= 0.1464466)
								AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Bottom);
							else if (pivot.y >= 0.8535534)
								AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Top);

							// adjust extender position
							var transformScreenPosition = camera.WorldToScreenPoint(transform.position);
							var bubblePosition = AttachedBubble.rectTransform.position;
							var bubbleRect = AttachedBubble.rectTransform.rect;
							var bubbleBorderInfo = AttachedBubble.ExtenderBorderInfo[(int)AttachedBubble.ExtenderDock];
							var v1 = 0F; var v2 = 0F; var o = 0F;
							switch( bubbleBorderInfo.Border )
							{
								case ExtenderBorderEnum.Bottom:
								case ExtenderBorderEnum.Top:
									v1 = bubblePosition.x + bubbleRect.xMin + bubbleBorderInfo.CutoffNear;
									v2 = bubblePosition.x + bubbleRect.xMax - bubbleBorderInfo.CutoffFar;
									o = transformScreenPosition.x.Remap(v1, v2, 0, 1);
									AttachedBubble.BindExtenderPosition(o);
									break;
								case ExtenderBorderEnum.Left:
								case ExtenderBorderEnum.Right:
									v1 = bubblePosition.y + bubbleRect.yMin + bubbleBorderInfo.CutoffNear;
									v2 = bubblePosition.y + bubbleRect.yMax - bubbleBorderInfo.CutoffFar;
									o = transformScreenPosition.y.Remap(v1, v2, 0, 1);
									AttachedBubble.BindExtenderPosition(o);
									break;
							}

						}
					}
				}
			}
		}

		

		
	}

}