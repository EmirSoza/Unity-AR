#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace LivelyTextGlyphs
{
    [CustomEditor(typeof(LTText))]
    public class LTTextEditor : Editor
    {
        new LTText target { get { return base.target as LTText; } }

        SerializedProperty[] Properties;

        void OnEnable()
        {
            Properties = new SerializedProperty[20]; int p = 0;
            Properties[p++] = serializedObject.FindProperty("Text");
            Properties[p++] = serializedObject.FindProperty("Font");
            Properties[p++] = serializedObject.FindProperty("FontSize");
            Properties[p++] = serializedObject.FindProperty("FontStyle");
            Properties[p++] = serializedObject.FindProperty("LineSpacing");
            Properties[p++] = serializedObject.FindProperty("RichText");

            Properties[p++] = serializedObject.FindProperty("Alignment");
            Properties[p++] = serializedObject.FindProperty("AlignByGeometry");
            Properties[p++] = serializedObject.FindProperty("HorizontalOverflow");
            Properties[p++] = serializedObject.FindProperty("VerticalOverflow");
            Properties[p++] = serializedObject.FindProperty("PageNumber");
            Properties[p++] = serializedObject.FindProperty("PageBreakChars");
            Properties[p++] = serializedObject.FindProperty("RaycastTarget");

            Properties[p++] = serializedObject.FindProperty("Color");
            Properties[p++] = serializedObject.FindProperty("ColorOverride");
            Properties[p++] = serializedObject.FindProperty("ColorBL");
            Properties[p++] = serializedObject.FindProperty("ColorTL");
            Properties[p++] = serializedObject.FindProperty("ColorTR");
            Properties[p++] = serializedObject.FindProperty("ColorBR");
            Properties[p++] = serializedObject.FindProperty("IconAtlas");

            //Properties[p++] = serializedObject.FindProperty("ShowDebugGlyphs");
            //Properties[p++] = serializedObject.FindProperty("ShowDebugWords");
            //Properties[p++] = serializedObject.FindProperty("ShowDebugLines");
            //Properties[p++] = serializedObject.FindProperty("ShowDebugBlock");
            //Properties[p++] = serializedObject.FindProperty("ShowDebugParse");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var text = target.Text;

            var font = target.Font;
            var fontSize = target.FontSize;
            var fontStyle = target.FontStyle;
            var lineSpacing = target.LineSpacing;
            var richText = target.RichText;

            var alignment = target.Alignment;
            var alignByGeometry = target.AlignByGeometry;
            var horizontalOverflow = target.HorizontalOverflow;
            var verticleOverflow = target.VerticalOverflow;
            //var pagination = target.Pagination;
            var pageNumber = target.PageNumber;

            var color = target.Color;
            var colorOverride = target.ColorOverride;
            var colorBL = target.ColorBL;
            var colorTL = target.ColorTL;
            var colorTR = target.ColorTR;
            var colorBR = target.ColorBR;
            var iconAtlas = target.IconAtlas;

			EditorGUILayout.Space();
			for (int i = 0; i < Properties.Length; i++)
                EditorGUILayout.PropertyField(Properties[i], true);
			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();

            target.PageNumber = Mathf.Clamp(target.PageNumber, 1, target.PageCount);

            // check for full rebuild
            if (text != target.Text ||
                font != target.Font ||
                fontSize != target.FontSize ||
                fontStyle != target.FontStyle ||
                lineSpacing != target.LineSpacing ||
                richText != target.RichText ||
                color != target.Color ||
                colorOverride != target.ColorOverride ||
                colorBL != target.ColorBL ||
                colorTL != target.ColorTL ||
                colorTR != target.ColorTR ||
                colorBR != target.ColorBR ||
                iconAtlas != target.IconAtlas)
            {
                target.ForceRebuild();
            }

            // check for rebuild blocks
            if (alignByGeometry != target.AlignByGeometry ||
                horizontalOverflow != target.HorizontalOverflow ||
                verticleOverflow != target.VerticalOverflow)
            {
                target.ForceBlocks();
            }

            // check for new layout
            if (alignment != target.Alignment)
            {
                target.ForceLayout();
            }

            // check for new page to show
            if (pageNumber != target.PageNumber)
            {
                target.ForceNewPage();
            }
        }
    }
}

#endif