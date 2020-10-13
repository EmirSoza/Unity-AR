using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphLine
    {
        // all word glyphs that make up the line
        public List<GlyphWord> Words { get; set; }

        // horizontal distance between all words
        public int advance { get; protected set; }

        // bottom left -> top right bounds 
        public Rect pixelBounds { get; protected set; }

        // bottom left -> top right bounds 
        public Rect normalBounds { get; protected set; }

        // local horizontal position
        public int localX { get; set; }

        // local vertical position
        public int localY { get; set; }

        // bottom left canvas position
        public Vector2 position { get; protected set; }

        public GlyphLine()
        {
            Words = new List<GlyphWord>();
        }

        public void AddWord(GlyphWord word, LTText parent)
        {
            var lastWord = Words.LastOrDefault();

            if (lastWord == null)
                word.localX = 0;
            else
                word.localX = lastWord.localX + lastWord.advance;

            Words.Add(word);

            RecalculateInternals(parent);
        }

        public void RemoveLastWord(LTText parent)
        {
            Words.RemoveAt(Words.Count - 1);
            RecalculateInternals(parent);
        }

        void RecalculateInternals(LTText parent)
        {
            if (Words.Count == 0)
            {
                advance = 0;
                pixelBounds = Rect.zero;
                normalBounds = Rect.zero;
            }
            else
            {
                advance = Words.Sum(w => w.advance);

                pixelBounds = new Rect()
                {
                    xMin = Words.Min(w => w.localX + w.pixelBounds.xMin),
                    xMax = Words.Max(w => w.localX + w.pixelBounds.xMax),
                    yMin = Words.Min(w => w.pixelBounds.yMin),
                    yMax = Words.Max(w => w.pixelBounds.yMax)
                };

                normalBounds = new Rect()
                {
                    xMin = 0,
                    yMin = 0,
                    xMax = advance,
                    yMax = parent.fontLineHeight - (parent.fontLineHeight - parent.fontLineAscent)
                };
            }
        }

        public void CalculatePosition(Vector2 parentPosition)
        {
            position =
                parentPosition +
                new Vector2(localX, localY);

            Words.ForEach(w => w.CalculatePosition(position));
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var w in Words)
                result.Append(w);
            return result.ToString();
        }
    }
}