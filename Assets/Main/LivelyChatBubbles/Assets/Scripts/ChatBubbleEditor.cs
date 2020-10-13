#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace LivelyChatBubbles
{
	[CustomPropertyDrawer(typeof(ExtenderBorderInfo))]
	public class ExtenderBorderDrawer : PropertyDrawer
	{
		static GUIContent
			GUIC_Margin = new GUIContent("    M"),         // margin
			GUIC_Near = new GUIContent("    N"),           // cutoff near
			GUIC_Far = new GUIContent("    F");            // cutoff far

		public static void ShowList(SerializedProperty list)
		{
			EditorGUILayout.PropertyField(list, new GUIContent("Border Info"));
			EditorGUI.indentLevel++;
			if (list.isExpanded)
				for (int i = 0; i < list.arraySize; i++)
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			EditorGUI.indentLevel--;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var p1 = property.FindPropertyRelative("Border");
			var p2 = property.FindPropertyRelative("Enabled");
			var p3 = property.FindPropertyRelative("Margin");
			var p4 = property.FindPropertyRelative("CutoffNear");
			var p5 = property.FindPropertyRelative("CutoffFar");

			label.text = p1.enumDisplayNames[p1.enumValueIndex];
			label = EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, label);
			var ind = EditorGUI.indentLevel; EditorGUI.indentLevel = 0;
			var lwd = EditorGUIUtility.labelWidth; EditorGUIUtility.labelWidth = 30;

			var col1Width = 20;
			var col234Width = (position.width - col1Width) / 3;

			position.width = col1Width;
			EditorGUI.PropertyField(position, p2, GUIContent.none);
			position.x += col1Width;
			position.width = col234Width;

			EditorGUI.PropertyField(position, p3, GUIC_Margin);
			position.x += col234Width;
			EditorGUI.PropertyField(position, p4, GUIC_Near);
			position.x += col234Width;
			EditorGUI.PropertyField(position, p5, GUIC_Far);

			EditorGUI.indentLevel = ind;
			EditorGUIUtility.labelWidth = lwd;
			EditorGUI.EndProperty();
		}
	}
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ChatBubble))]
	public class ChatBubbleEditor : Editor
	{
		new ChatBubble target { get { return base.target as ChatBubble; } }

		SerializedProperty PAutoSize;

		//SerializedProperty PNameMinimized;
		SerializedProperty PNameComponent;
		SerializedProperty PNameValue;
		
		SerializedProperty PMessageComponent;
		SerializedProperty PMessageValue;
		SerializedProperty PMessageMinimumSize;
		SerializedProperty PMessageWrapWidth;
		
		SerializedProperty PExtenderComponent;
		SerializedProperty PExtenderBorderInfo;
		SerializedProperty PExtenderDock;
		SerializedProperty PExtenderPosition;

		static Color BorderColorEnabled = new Color(0.2f, 1, 0.2f);
		static Color BorderColorDisabled = new Color(0.4f, 0.4f, 0.4f);
		static Color MinimumFill = new Color(0.2f, 1, 0.2f, 0.05f);
		static Color MinimumColor = new Color(0.2f, 1, 0.2f);
		static Color WrapColor = new Color(1, 0.2f, 1);

		private void OnEnable()
		{
			PNameComponent = serializedObject.FindProperty("NameComponent");
			PNameValue = serializedObject.FindProperty("NameValue");

			PMessageComponent = serializedObject.FindProperty("MessageComponent");
			PMessageValue = serializedObject.FindProperty("MessageValue");
			PMessageMinimumSize = serializedObject.FindProperty("MessageMinimumSize");
			PMessageWrapWidth = serializedObject.FindProperty("MessageWrapWidth");

			PExtenderComponent = serializedObject.FindProperty("ExtenderComponent");
			PExtenderBorderInfo = serializedObject.FindProperty("ExtenderBorderInfo");
			PExtenderDock = serializedObject.FindProperty("ExtenderDock");
			PExtenderPosition = serializedObject.FindProperty("ExtenderPosition");

			PAutoSize = serializedObject.FindProperty("AutoSize");
		}

		public override void OnInspectorGUI()
		{
			var priorExtenderDock = target.ExtenderDock;
			var priorExtenderPosition = target.ExtenderPosition;

			serializedObject.Update();

			ChatBubble.BindingsExpanded = EditorGUILayout.Foldout(ChatBubble.BindingsExpanded, "Bindings");
			if (ChatBubble.BindingsExpanded )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(PNameComponent, new GUIContent("Name"));
				EditorGUILayout.PropertyField(PMessageComponent, new GUIContent("Message"));
				EditorGUI.indentLevel--;
			}

			ChatBubble.ExtenderExpanded = EditorGUILayout.Foldout(ChatBubble.ExtenderExpanded, "Extender");
			if (ChatBubble.ExtenderExpanded)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(PExtenderComponent, new GUIContent("Component"));
				ExtenderBorderDrawer.ShowList(PExtenderBorderInfo);
				EditorGUILayout.PropertyField(PExtenderDock, new GUIContent("Dock"));
				EditorGUILayout.PropertyField(PExtenderPosition, new GUIContent("Position"));
				EditorGUI.indentLevel--;
			}

			ChatBubble.ValuesExpanded = EditorGUILayout.Foldout(ChatBubble.ValuesExpanded, "Values");
			if (ChatBubble.ValuesExpanded)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(PNameValue, new GUIContent("Name"));
				EditorGUILayout.PropertyField(PMessageValue, new GUIContent("Message"));
				EditorGUILayout.PropertyField(PAutoSize);
				EditorGUILayout.PropertyField(PMessageWrapWidth, new GUIContent("Wrap Width"));
				EditorGUILayout.PropertyField(PMessageMinimumSize, new GUIContent("Minimum Size"));
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();

			if (target.BindNameValue(target.NameValue))
				EditorUtility.SetDirty(target.NameComponent);

			if (target.BindMessageValue(target.MessageValue))
				EditorUtility.SetDirty(target.MessageComponent);

			if (target.AutoSize)
				target.PerformAutoSize();

			if (priorExtenderDock != target.ExtenderDock)
			{
				target.PerformExtenderSnap();
				target.PerformExtenderPosition();
			}

			if (priorExtenderPosition != target.ExtenderPosition)
				target.PerformExtenderPosition();

			SceneView.RepaintAll();
		}

		private void OnSceneGUI()
		{
			// draw borders for extender info
			if (target.ExtenderComponent)
			{
				Handles.matrix = target.rectTransform.localToWorldMatrix;

				// calculate edges
				Vector3 b1, b2, l1, l2, r1, r2, t1, t2; b1 = b2 = l1 = l2 = r1 = r2 = t1 = t2 = Vector3.zero;
				CalculateEditorBorderVertices(target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Top], ref t1, ref t2);
				CalculateEditorBorderVertices(target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Left], ref l1, ref l2);
				CalculateEditorBorderVertices(target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Right], ref r1, ref r2);
				CalculateEditorBorderVertices(target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Bottom], ref b1, ref b2);

				// draw edges
				Handles.color = target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Top].Enabled ? BorderColorEnabled : BorderColorDisabled; Handles.DrawLine(t1, t2);
				Handles.color = target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Left].Enabled ? BorderColorEnabled : BorderColorDisabled; Handles.DrawLine(l1, l2);
				Handles.color = target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Right].Enabled ? BorderColorEnabled : BorderColorDisabled; Handles.DrawLine(r1, r2);
				Handles.color = target.ExtenderBorderInfo[(int)ExtenderBorderEnum.Bottom].Enabled ? BorderColorEnabled : BorderColorDisabled; Handles.DrawLine(b1, b2);

				// draw between edges
				Handles.color = BorderColorDisabled;
				Handles.DrawLine(b1, l1);
				Handles.DrawLine(l2, t1);
				Handles.DrawLine(t2, r2);
				Handles.DrawLine(r1, b2);
			}

			// draw message area lines
			if ( target.MessageComponent && target.AutoSize )
			{
				Handles.matrix = target.MessageComponent.rectTransform.localToWorldMatrix;
				Vector3 p1, p2; p1 = p2 = Vector3.zero;
				var innerRect = target.MessageComponent.rectTransform.rect;

				// minimum size
				Handles.DrawSolidRectangleWithOutline(new Rect(new Vector2(innerRect.xMin, innerRect.yMin), target.MessageMinimumSize), MinimumFill, MinimumColor);

				// wrap line
				p1.x = p2.x = innerRect.xMin + target.MessageWrapWidth;
				p1.y = innerRect.yMin;
				p2.y = innerRect.yMax;
				Handles.color = WrapColor;
				Handles.DrawLine(p1, p2);
			}
		}

		public void CalculateEditorBorderVertices(ExtenderBorderInfo info, ref Vector3 v1, ref Vector3 v2)
		{
			var rect = target.rectTransform.rect;
			v1.z = v2.z = target.rectTransform.localPosition.z;
			switch (info.Border)
			{
				case ExtenderBorderEnum.Bottom:
					v1.x = rect.xMin + info.CutoffNear;
					v2.x = rect.xMax - info.CutoffFar;
					v1.y = v2.y = rect.yMin + info.Margin;
					break;

				case ExtenderBorderEnum.Left:
					v1.y = rect.yMin + info.CutoffNear;
					v2.y = rect.yMax - info.CutoffFar;
					v1.x = v2.x = rect.xMin + info.Margin;
					break;

				case ExtenderBorderEnum.Right:
					v1.y = rect.yMin + info.CutoffNear;
					v2.y = rect.yMax - info.CutoffFar;
					v1.x = v2.x = rect.xMax - info.Margin;
					break;

				case ExtenderBorderEnum.Top:
					v1.x = rect.xMin + info.CutoffNear;
					v2.x = rect.xMax - info.CutoffFar;
					v1.y = v2.y = rect.yMax - info.Margin;
					break;
			}
		}

	}
}

#endif
