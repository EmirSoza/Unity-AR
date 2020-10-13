using System;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphAnimShake : GlyphAnim
    {
		// radius offset when shaking
        public float Radius = 1f;

		// number of shakes per second
        public float ShakesPerSecond = 1f;

		// internal shake time 
        float nextShakeTime = 0f;

        public GlyphAnimShake()
        {
            tagName = "shake";
        }

        private void OnEnable()
        {
            nextShakeTime = Time.time;
        }

        private void OnDisable()
        {
            for (int i = 0; i < glyphs.Count; i++)
                glyphs[i].offset = Vector3.zero;
            SetDirty();
        }

        protected override void GlyphRemoved(Glyph glyph)
        {
            glyph.offset = Vector3.zero;
            SetDirty();
        }

        private void Update()
        {
            var time = Time.time;
            if (time < nextShakeTime || ShakesPerSecond <= 0)
                return;
			
            nextShakeTime = time + 1 / ShakesPerSecond;

            var angle = UnityEngine.Random.Range(0, (float)Math.PI * 2);
            var offset = new Vector3(
                (float)Math.Cos(angle) * Radius * 0.5f,
                (float)Math.Sin(angle) * Radius * 0.5f,
                0);

            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].offset = offset * glyphs[i].style.getFontSize();

            SetDirty();
        }
    }
}