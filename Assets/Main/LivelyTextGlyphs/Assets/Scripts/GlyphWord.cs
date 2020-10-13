using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphWord
    {
        // all glyphs that make up this word
        public List<Glyph> Glyphs { get; set; }

        // distance between this word and the next
        public int advance { get; protected set; }

        // bottom left -> top right bounds 
        public Rect pixelBounds { get; protected set; }

        // local horizontal position
        public int localX { get; set; }

        // bottom left position on the canvas
        public Vector2 position { get; protected set; }

        public bool IsWhiteSpace
        {
            get { return Glyphs.Count == 1 && Glyphs[0] is SpaceGlyph; }
        }

        public bool IsReturn
        {
            get { return Glyphs.Count == 1 && Glyphs[0] is ReturnGlyph; }
        }

        public GlyphWord()
        {
            Glyphs = new List<Glyph>();
        }

        public void AddGlyph(Glyph glyph)
        {
            var lastGlyph = Glyphs.LastOrDefault();

            if (lastGlyph == null)
                glyph.localX = 0;
            else
                glyph.localX = lastGlyph.localX + lastGlyph.advance;

            Glyphs.Add(glyph);

            RecalculateInternals();
        }

        public void RemoveLastGlyph()
        {
            Glyphs.RemoveAt(Glyphs.Count - 1);

            RecalculateInternals();
        }

        void RecalculateInternals()
        {
            advance = Glyphs.Sum(g => g.advance);

            pixelBounds = new Rect()
            {
                xMin = Glyphs.Min(g => g.localX + g.pixelBounds.xMin),
                xMax = Glyphs.Max(g => g.localX + g.pixelBounds.xMax),
                yMin = Glyphs.Min(g => g.pixelBounds.yMin),
                yMax = Glyphs.Max(g => g.pixelBounds.yMax)
            };
        }

        public float normalizedWidth { get { return advance; } }
        public float pixelizedWidth { get { return pixelBounds.width; } }

        public void CalculatePosition(Vector2 parentPosition)
        {
            position =
                parentPosition +
                new Vector2(localX, 0);

            Glyphs.ForEach(g => g.CalculatePosition(position));
        }

        // list of glyphs that make up the block
        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var g in Glyphs)
                result.Append(g);
            return result.ToString();
        }
    }
}
