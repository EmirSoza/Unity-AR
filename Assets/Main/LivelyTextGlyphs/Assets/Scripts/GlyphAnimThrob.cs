using System;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphAnimThrob : GlyphAnim
    {
		// throb hue applied to glyphs
        public Color hue = Color.white;

		// speed at which the throb occurs
        public float Speed = 1f;

		// frequency at which the throb occurs
        public float Frequency = 0f;

        public GlyphAnimThrob()
        {
            tagName = "throb";
        }

        private void OnValidate()
        {
            hue = new Color(hue.r, hue.g, hue.b, 0);
        }

        private void OnDisable()
        {
            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].hue = new Color(hue.r, hue.g, hue.b, 0);
        }

        protected override void GlyphRemoved(Glyph glyph)
        {
            glyph.hue = new Color(hue.r, hue.g, hue.b, 0);
            SetDirty();
        }

        private void Update()
        {
            // update offsets
            var time = Time.time;
            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].hue = new Color(hue.r, hue.g, hue.b, ((float)Math.Cos((i * Frequency + time * Speed) * Mathf.PI * 2) + 1f) / 2f);
            SetDirty();
        }
    }
}


