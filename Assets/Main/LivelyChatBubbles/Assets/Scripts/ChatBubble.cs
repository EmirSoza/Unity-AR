using System;
using UnityEngine;
using UnityEngine.UI;

namespace LivelyChatBubbles
{
	[Serializable]
	public enum ExtenderBorderEnum : int
	{
		Bottom		= 0,
		Left		= 1,
		Right		= 2,
		Top			= 3
	}

	[Serializable]
	public class ExtenderBorderInfo
	{
		public ExtenderBorderEnum Border;

		[Tooltip("True if the border allows the extender to be visible.")]
		public bool Enabled = true;

		[Tooltip("Offset in pixels from the edge towards center for the extender to travel along.  This is necessary for images that have spacing and shadows around their edges.")]
		public float Margin;

		[Tooltip("Offset in pixels on the near side of the edge for the extender to travel along.  This is necessary so the extender does not move where the image may have rounded corners.")]
		public float CutoffNear;

		[Tooltip("Offset in pixels on the far side of the edge for the extender to travel along.  This is necessary so the extender does not move where the image may have rounded corners.")]
		public float CutoffFar;
	}
	
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ChatBubble : MonoBehaviour
	{
		[Tooltip("Text component in the tree used to display the bubble's name.")]
		public Text NameComponent;

		public string NameValue;
		public bool BindNameValue( string value )
		{
			NameValue = value;
			if (!NameComponent || NameComponent.text == value)
				return false;
			NameComponent.text = value;
			return true;
		}

		[Tooltip("Text component in the tree used to display the bubble's message.")]
		public Text MessageComponent;

		[Multiline]
		public string MessageValue;
		public bool BindMessageValue(string value)
		{
			MessageValue = value;
			if (!MessageComponent || MessageComponent.text == value)
				return false;
			MessageComponent.text = value;
			if ( AutoSize )
				PerformAutoSize();
			return true;
		}

		[Tooltip("True if the bubble should be autosized according to the message content.")]
		public bool AutoSize;
		
		[Tooltip("Minimum size required.")]
		public Vector2 MessageMinimumSize = new Vector2(60, 30);

		[Tooltip("Maximum width before wrapping.")]
		public float MessageWrapWidth = 300;
		
		[Tooltip("The image should be aligned to the top without any empty pixel rows/columns around it.")]
		public Image ExtenderComponent;

		[Tooltip("Configurable borders that define where the extender can travel.")]
		public ExtenderBorderInfo[] ExtenderBorderInfo;

		[Tooltip("Border to dock the extender to.")]
		public ExtenderBorderEnum ExtenderDock = ExtenderBorderEnum.Bottom;
		public bool BindExtenderDock( ExtenderBorderEnum value )
		{
			if (ExtenderDock == value)
				return false;
			ExtenderDock = value;
			PerformExtenderSnap();
			PerformExtenderPosition();
			return true;
		}

		[Range(0, 1)]
		[Tooltip("Offset of the extender along the docked edge.")]
		public float ExtenderPosition = 0.5f;
		public bool BindExtenderPosition( float value )
		{
			value = Mathf.Clamp01(value);
			if (ExtenderPosition == value)
				return false;
			ExtenderPosition = value;
			PerformExtenderPosition();
			return true;
		}

		private RectTransform _rectTransform;
		public RectTransform rectTransform { get { if (!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

		public static bool BindingsExpanded;
		public static bool ValuesExpanded;
		public static bool ExtenderExpanded;
		static Vector3 mirrorX = new Vector3(-1, 1, 1);

		void Awake()
		{
			// default border info for each border
			if (ExtenderBorderInfo == null)
			{
				ExtenderBorderInfo = new ExtenderBorderInfo[4];
				ExtenderBorderInfo[(int)ExtenderBorderEnum.Bottom] = new ExtenderBorderInfo() { Border = ExtenderBorderEnum.Bottom };
				ExtenderBorderInfo[(int)ExtenderBorderEnum.Left] = new ExtenderBorderInfo() { Border = ExtenderBorderEnum.Left };
				ExtenderBorderInfo[(int)ExtenderBorderEnum.Right] = new ExtenderBorderInfo() { Border = ExtenderBorderEnum.Right };
				ExtenderBorderInfo[(int)ExtenderBorderEnum.Top] = new ExtenderBorderInfo() { Border = ExtenderBorderEnum.Top };
			}
		}

		#region AutoSizeFunctionality

		public Vector2 PerformManualSize( string value )
		{
			var originalMinimumSize = MessageMinimumSize;

			// adjust size to match size for incoming value
			if (MessageComponent)
			{
				var originalMessageValue = MessageComponent.text;

				// resize
				MessageComponent.text = value;
				PerformAutoSize();
				MessageMinimumSize = MessageComponent.rectTransform.rect.size;

				// restore old value
				MessageComponent.text = originalMessageValue;
			}

			return originalMinimumSize;
		}

		public void PerformAutoSize()
		{
			if (!MessageComponent)
				return;
			PerformAutoHeight();
			if (PerformAutoWidth())
				PerformAutoHeight();
		}

		bool PerformAutoHeight()
		{
			var curHeight = MessageComponent.rectTransform.rect.height;
			var nxtHeight = Mathf.Max(MessageMinimumSize.y, LayoutUtility.GetPreferredHeight(MessageComponent.rectTransform));
			if (curHeight == nxtHeight)
				return false;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + (nxtHeight - curHeight));
			return true;
		}

		bool PerformAutoWidth()
		{
			var curWidth = MessageComponent.rectTransform.rect.width;
			var nxtWidth = Mathf.Max(MessageMinimumSize.x, Mathf.Min(MessageWrapWidth, LayoutUtility.GetPreferredWidth(MessageComponent.rectTransform)));
			if (curWidth == nxtWidth)
				return false;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width + (nxtWidth - curWidth));
			return true;
		}

		#endregion

		#region ExtenderFunctionality

		public void PerformExtenderSnap()
		{
			if (ExtenderComponent)
			{
				ExtenderComponent.rectTransform.pivot = new Vector2(0.5f, 1f);

				switch (ExtenderDock)
				{
					case ExtenderBorderEnum.Bottom:
						ExtenderComponent.rectTransform.anchorMin = ExtenderComponent.rectTransform.anchorMax = new Vector2(0.5f, 0f);
						ExtenderComponent.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
						break;

					case ExtenderBorderEnum.Left:
						ExtenderComponent.rectTransform.anchorMin = ExtenderComponent.rectTransform.anchorMax = new Vector2(0f, 0.5f);
						ExtenderComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, -90);
						break;

					case ExtenderBorderEnum.Right:
						ExtenderComponent.rectTransform.anchorMin = ExtenderComponent.rectTransform.anchorMax = new Vector2(1f, 0.5f);
						ExtenderComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, 90);
						break;

					case ExtenderBorderEnum.Top:
						ExtenderComponent.rectTransform.anchorMin = ExtenderComponent.rectTransform.anchorMax = new Vector2(0.5f, 1f);
						ExtenderComponent.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
						break;
				}

				if (ExtenderComponent.enabled && !ExtenderBorderInfo[(int)ExtenderDock].Enabled)
					ExtenderComponent.enabled = false;
				else if (!ExtenderComponent.enabled && ExtenderBorderInfo[(int)ExtenderDock].Enabled)
					ExtenderComponent.enabled = true;
			}
		}

		public void PerformExtenderPosition()
		{
			if (ExtenderComponent)
			{
				var v1 = Vector3.zero;
				var v2 = Vector3.zero;
				var info = ExtenderBorderInfo[(int)ExtenderDock];
				CalculateExtenderBorderVertices(info, ref v1, ref v2);

				var half = 0f;
				switch (ExtenderDock)
				{
					case ExtenderBorderEnum.Bottom:
						half = ExtenderComponent.rectTransform.rect.width * 0.5f;
						ExtenderComponent.rectTransform.anchoredPosition = new Vector3(ExtenderPosition.Remap(0, 1, v1.x + half, v2.x - half), info.Margin, 0);
						ExtenderComponent.rectTransform.localScale = ExtenderPosition <= 0.5f ? Vector3.one : mirrorX;
						break;

					case ExtenderBorderEnum.Left:
						half = ExtenderComponent.rectTransform.rect.height * 0.5f;
						ExtenderComponent.rectTransform.anchoredPosition = new Vector3(info.Margin, ExtenderPosition.Remap(0, 1, v1.y + half, v2.y - half), 0);
						ExtenderComponent.rectTransform.localScale = ExtenderPosition <= 0.5f ? mirrorX : Vector3.one;
						break;

					case ExtenderBorderEnum.Right:
						half = ExtenderComponent.rectTransform.rect.height * 0.5f;
						ExtenderComponent.rectTransform.anchoredPosition = new Vector3(-info.Margin, ExtenderPosition.Remap(0, 1, v1.y + half, v2.y - half), 0);
						ExtenderComponent.rectTransform.localScale = ExtenderPosition <= 0.5f ? Vector3.one : mirrorX;
						break;

					case ExtenderBorderEnum.Top:
						half = ExtenderComponent.rectTransform.rect.width * 0.5f;
						ExtenderComponent.rectTransform.anchoredPosition = new Vector3(ExtenderPosition.Remap(0, 1, v1.x + half, v2.x - half), -info.Margin, 0);
						ExtenderComponent.rectTransform.localScale = ExtenderPosition <= 0.5f ? mirrorX : Vector3.one;
						break;
				}
			}
		}

		public void CalculateExtenderBorderVertices(ExtenderBorderInfo info, ref Vector3 v1, ref Vector3 v2)
		{
			v1.z = v2.z = rectTransform.localPosition.z;
			var half = rectTransform.sizeDelta / 2;
			switch (info.Border)
			{
				case ExtenderBorderEnum.Bottom:
					v1.x = -half.x + info.CutoffNear;
					v2.x = half.x - info.CutoffFar;
					v1.y = v2.y = -half.y + info.Margin;
					break;

				case ExtenderBorderEnum.Left:
					v1.y = -half.y + info.CutoffNear;
					v2.y = half.y - info.CutoffFar;
					v1.x = v2.x = -half.x + info.Margin;
					break;

				case ExtenderBorderEnum.Right:
					v1.y = -half.y + info.CutoffNear;
					v2.y = half.y - info.CutoffFar;
					v1.x = v2.x = half.x - info.Margin;
					break;

				case ExtenderBorderEnum.Top:
					v1.x = -half.x + info.CutoffNear;
					v2.x = half.x - info.CutoffFar;
					v1.y = v2.y = half.y - info.Margin;
					break;
			}
		}


		#endregion

	}

}