using System.Collections.Generic;
using UnityEngine;

namespace LivelyTextGlyphs
{
    [RequireComponent(typeof(LTText))]
    public abstract class GlyphAnim : MonoBehaviour
    {
        // matching tag name
        public string tagName;

        // glyphs to animate
        protected List<Glyph> glyphs { get; private set; }

        // required component access
        LTText _text; LTText text { get { if (!_text) _text = GetComponent<LTText>(); return _text; } }

		// true when inside graphic rebuild to help suppress editor errors while rebuilding text
        public static bool insideGraphicRebuild;

        public GlyphAnim()
        {
            glyphs = new List<Glyph>();
        }

        public void ClearGlyphs()
        {
            glyphs.Clear();
            for (var i = 0; i < glyphs.Count; i++)
                GlyphRemoved(glyphs[i]);
        }

        public void AddGlyph(Glyph glyph)
        {
            glyphs.Add(glyph);
            GlyphAdded(glyph);
        }

        protected void SetDirty()
        {
            if (!insideGraphicRebuild)
                text.SetVerticesDirty();
        }

        protected virtual void GlyphAdded(Glyph glyph) { }
        protected virtual void GlyphRemoved(Glyph glyph) { }
    }
}