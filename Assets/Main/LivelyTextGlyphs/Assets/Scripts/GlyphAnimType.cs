using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class GlyphAnimType : GlyphAnim
    {
		// number of glyphs to output per second
        public int GlyphsPerSecond = 100;

		// pause time in seconds when outputting configured short pause characters
        public float ShortPauseLength = 0.1f;

		// pause time in seconds when outputted configured long pause characters
        public float LongPauseLength = 0.25f;

		// characters that generate a short pause
        public List<char> ShortPauseChars = new List<char> { ',', ';', ':' };

		// characters that generate a long pause
        public List<char> LongPauseChars = new List<char> { '.', '!', '?' };

        public GlyphAnimType()
        {
            tagName = "type";
        }

        private void OnEnable()
        {
            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].visible = false;
            SetDirty();
            StartCoroutine("Execute");
        }

        private void OnDisable()
        {
            StopCoroutine("Execute");
            for (var i = 0; i < glyphs.Count; i++)
                glyphs[i].visible = true;
            SetDirty();
        }

        protected override void GlyphAdded(Glyph glyph)
        {
            if (enabled)
            {
                glyph.visible = false;
                SetDirty();
            }
        }

        protected override void GlyphRemoved(Glyph glyph)
        {
            glyph.visible = true;
            SetDirty();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                enabled = false;
        }

        IEnumerator Execute()
        {
            // precise characters to output this frame (can be decimal)
            var preciseFrameChars = 0f;
            // whole characters to output this frame (non decimal)
            var wholeFrameChars = 0;
            // whole characters that were actually output this frame (non decimal)
            var outputFrameChars = 0;
            // next character index to output
            var nextGlyphIndex = 0;
            // short pause detected
            var shortPause = false;
            // long pause detected
            var longPause = false;
            // next glyph reference
            Glyph nextGlyph = null;

            while (true)
            {
                // reset
                shortPause = false;
                longPause = false;
                // timing this frame
                preciseFrameChars += Time.deltaTime * GlyphsPerSecond;
                wholeFrameChars = (int)preciseFrameChars;
                // remove whole chars from time buffer to bring it back to < 1
                preciseFrameChars = wholeFrameChars == 0 ? preciseFrameChars : preciseFrameChars % wholeFrameChars;
                // process output chars
                for (outputFrameChars = 0; outputFrameChars < wholeFrameChars; outputFrameChars++)
                    if (nextGlyphIndex + outputFrameChars < glyphs.Count)
                    {
                        // pull reference
                        nextGlyph = glyphs[nextGlyphIndex + outputFrameChars];
                        // visible
                        nextGlyph.visible = true;
                        // check for pauses
                        shortPause = nextGlyph is CharGlyph && ShortPauseChars.Contains((nextGlyph as CharGlyph).id);
                        longPause = nextGlyph is CharGlyph && LongPauseChars.Contains((nextGlyph as CharGlyph).id);
                        // stop if pauses detected
                        if (shortPause || longPause)
                            break;
                    }
                    else
                        break;

                // update glyph index based on actual output this frame
                nextGlyphIndex += outputFrameChars + (shortPause || longPause ? 1 : 0);
                if (outputFrameChars > 0)
                    SetDirty();

                // check for completion
                if (nextGlyphIndex >= glyphs.Count)
                    break;

                // clear time buffer if there is a pause
                if (shortPause || longPause)
                    preciseFrameChars = 0;

                // wait
                if (shortPause)
                    yield return new WaitForSeconds(ShortPauseLength);
                else if (longPause)
                    yield return new WaitForSeconds(LongPauseLength);
                else
                    yield return new WaitForEndOfFrame();
            }

            // disable the component when finished
            enabled = false;
        }
    }
}