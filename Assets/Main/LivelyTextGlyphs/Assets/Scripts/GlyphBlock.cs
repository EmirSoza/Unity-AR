using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphBlock
    {
        // all line glyphs that make up the block
        public List<GlyphLine> Lines { get; set; }

        // bottom left -> top right bounds 
        public Rect pixelBounds { get; protected set; }

        // bottom left -> top right bounds 
        public Rect normalBounds { get; protected set; }

        // bottom left canvas position
        public Vector2 position { get; protected set; }

        public GlyphBlock()
        {
            Lines = new List<GlyphLine>();
        }

        public void AddLine(GlyphLine line, LTText parent)
        {
            var lastLine = Lines.LastOrDefault();

            if (lastLine == null)
                line.localY = -parent.fontLineAscent;
            else
                line.localY = lastLine.localY - parent.fontLineHeight;

            // offset local x based on alignment 
            if (parent.Alignment == TextAnchor.UpperLeft || parent.Alignment == TextAnchor.MiddleLeft || parent.Alignment == TextAnchor.LowerLeft)
            {
                if (!parent.AlignByGeometry)
                    line.localX = 0;
                else
                    line.localX = -(int)line.pixelBounds.xMin;
            }
            else if (parent.Alignment == TextAnchor.UpperCenter || parent.Alignment == TextAnchor.MiddleCenter || parent.Alignment == TextAnchor.LowerCenter)
            {
                if (!parent.AlignByGeometry)
                    line.localX = (int)(parent.pixelPerfectBounds.width / 2 - line.normalBounds.width / 2);
                else
                    line.localX = (int)(parent.pixelPerfectBounds.width / 2 - line.pixelBounds.width / 2 - line.pixelBounds.xMin);
            }
            else if (parent.Alignment == TextAnchor.UpperRight || parent.Alignment == TextAnchor.MiddleRight || parent.Alignment == TextAnchor.LowerRight)
            {
                if (!parent.AlignByGeometry)
                    line.localX = (int)(parent.pixelPerfectBounds.width - line.normalBounds.width);
                else
                    line.localX = (int)(parent.pixelPerfectBounds.width - line.pixelBounds.width - line.pixelBounds.xMin);
            }

            Lines.Add(line);

            RecalculateInternals(parent);
        }

        public void AdjustLineHorizontalOffset(LTText parent)
        {
            for (var i = 0; i < Lines.Count; i++)
            {
                // offset local x based on alignment 
                if (parent.Alignment == TextAnchor.UpperLeft || parent.Alignment == TextAnchor.MiddleLeft || parent.Alignment == TextAnchor.LowerLeft)
                {
                    if (!parent.AlignByGeometry)
                        Lines[i].localX = 0;
                    else
                        Lines[i].localX = -(int)Lines[i].pixelBounds.xMin;
                }
                else if (parent.Alignment == TextAnchor.UpperCenter || parent.Alignment == TextAnchor.MiddleCenter || parent.Alignment == TextAnchor.LowerCenter)
                {
                    if (!parent.AlignByGeometry)
                        Lines[i].localX = (int)(parent.pixelPerfectBounds.width / 2 - Lines[i].normalBounds.width / 2);
                    else
                        Lines[i].localX = (int)(parent.pixelPerfectBounds.width / 2 - Lines[i].pixelBounds.width / 2 - Lines[i].pixelBounds.xMin);
                }
                else if (parent.Alignment == TextAnchor.UpperRight || parent.Alignment == TextAnchor.MiddleRight || parent.Alignment == TextAnchor.LowerRight)
                {
                    if (!parent.AlignByGeometry)
                        Lines[i].localX = (int)(parent.pixelPerfectBounds.width - Lines[i].normalBounds.width);
                    else
                        Lines[i].localX = (int)(parent.pixelPerfectBounds.width - Lines[i].pixelBounds.width - Lines[i].pixelBounds.xMin);
                }
            }
            RecalculateInternals(parent);
        }

        public void RemoveLastLine(LTText parent)
        {
            Lines.RemoveAt(Lines.Count - 1);
            RecalculateInternals(parent);
        }

        void RecalculateInternals(LTText parent)
        {
            if (Lines.Count == 0)
            {
                pixelBounds = Rect.zero;
                normalBounds = Rect.zero;
            }
            else
            {
                // recalculate pixel bounds
                pixelBounds = new Rect()
                {
                    xMin = Lines.Min(l => l.localX + l.pixelBounds.xMin),
                    xMax = Lines.Max(l => l.localX + l.pixelBounds.xMax),
                    yMin = Lines.Max(l => l.localY + l.pixelBounds.yMax),
                    yMax = Lines.Min(l => l.localY + l.pixelBounds.yMin)
                };

                // recalculate normal bounds
                normalBounds = new Rect()
                {
                    xMin = Lines.Min(l => l.localX + l.normalBounds.xMin),
                    yMin = 0,
                    xMax = Lines.Max(l => l.localX + l.normalBounds.xMax),
                    yMax = -(parent.fontLineAscent + (Lines.Count() - 1) * parent.fontLineHeight)
                };
            }
        }

        public void CalculatePosition(LTText parent)
        {
            var y = 0f;

            // adjust block for vertical alignment
            if (parent.Alignment == TextAnchor.UpperCenter || parent.Alignment == TextAnchor.UpperLeft || parent.Alignment == TextAnchor.UpperRight)
            {
                if (!parent.AlignByGeometry)
                    y = parent.pixelPerfectBounds.yMax;
                else
                    y = parent.pixelPerfectBounds.yMax - pixelBounds.yMin;
            }
            else if (parent.Alignment == TextAnchor.MiddleCenter || parent.Alignment == TextAnchor.MiddleLeft || parent.Alignment == TextAnchor.MiddleRight)
            {
                if (!parent.AlignByGeometry)
                    y = parent.pixelPerfectBounds.yMin + (parent.pixelPerfectBounds.height / 2) - (normalBounds.height / 2);
                else
                    y = parent.pixelPerfectBounds.yMin + (parent.pixelPerfectBounds.height / 2) - (pixelBounds.height / 2) - pixelBounds.yMin;
            }
            else if (parent.Alignment == TextAnchor.LowerCenter || parent.Alignment == TextAnchor.LowerLeft || parent.Alignment == TextAnchor.LowerRight)
            {
                if (!parent.AlignByGeometry)
                    y = parent.pixelPerfectBounds.yMin - normalBounds.height;
                else
                    y = parent.pixelPerfectBounds.yMin - pixelBounds.height - pixelBounds.yMin;
            }

            position = new Vector2(parent.pixelPerfectBounds.xMin, y);

            foreach (var l in Lines)
                l.CalculatePosition(position);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var l in Lines)
            {
                result.Append(l);
                result.Append("\r\n");
            }
            return result.ToString();
        }
    }
}