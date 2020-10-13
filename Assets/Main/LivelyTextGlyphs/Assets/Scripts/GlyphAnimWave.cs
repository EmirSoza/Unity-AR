using System;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphAnimWave : GlyphAnim
    {
		// wave height
        public float Height = 1f;

		// wave speed
        public float Speed = 1f;

		// wave frequency
        public float Frequency = 1f;

        public GlyphAnimWave()
        {
            tagName = "wave";
        }

        private void OnDisable()
        {
            for (int i = 0; i < glyphs.Count; i++)
                glyphs[i].offset = Vector3.zero;
            SetDirty();
        }

        protected override void GlyphAdded(Glyph glyph)
        {
        }

        protected override void GlyphRemoved(Glyph glyph)
        {
            glyph.offset = Vector3.zero;
            SetDirty();
        }

        private void Update()
        {
            // update offsets
            var time = Time.time;
            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].offset = new Vector3(0, (float)Math.Sin((i * Frequency + time * Speed) * Mathf.PI * 2) * Height * glyphs[i].style.getFontSize() * 0.5f, 0);
            SetDirty();
        }
    }
}