using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace LivelyTextGlyphs
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class LTText : MaskableGraphic, ILayoutElement
    {
        [Multiline]
        public string Text;

        [Header("Character")]
        public Font Font;
        public int FontSize = 24;
        public FontStyle FontStyle = FontStyle.Normal;
        public float LineSpacing = 1f;
        public bool RichText;

        [Header("Paragraph")]
        public TextAnchor Alignment = TextAnchor.UpperLeft;
        public bool AlignByGeometry = false;
        public HorizontalWrapMode HorizontalOverflow = HorizontalWrapMode.Wrap;
        public VerticalWrapMode VerticalOverflow = VerticalWrapMode.Truncate;

        public int PageNumber;
        public int PageCount { get { return blocks != null ? blocks.Count : 0; } }
        public List<char> PageBreakChars = new List<char> { '.', '!', '?' };

		bool HasBlocks { get { return PageNumber > 0 && blocks != null && blocks.Count > 0; } }

        public bool RaycastTarget;
        public override bool raycastTarget { get { return RaycastTarget; } set { RaycastTarget = value; } }

        // pixel perfect rectTransform.rect bounds
        [NonSerialized]
        public Rect pixelPerfectBounds = Rect.zero;

        [NonSerialized]
        public int fontLineHeight;

        [NonSerialized]
        public int fontLineAscent;

        [Header("Coloring")]
        public Color Color = Color.white;
        public override Color color { get { return Color; } set { Color = value; } }

        public bool ColorOverride = false;
        public Color ColorBL = Color.white;
        public Color ColorTL = Color.white;
        public Color ColorTR = Color.white;
        public Color ColorBR = Color.white;

		[Header("Iconography")]
		public SpriteAtlas IconAtlas;

        [Header("Debug")]
        public bool ShowDebugGlyphs;
        public bool ShowDebugWords;
        public bool ShowDebugLines;
        public bool ShowDebugBlock;
        public bool ShowDebugParse;

        public float minWidth { get; set; }
        public float preferredWidth { get; set; }
        public float flexibleWidth { get; set; }

        public float minHeight { get; set; }
        public float preferredHeight { get; set; }
        public float flexibleHeight { get; set; }

        public int layoutPriority { get; set; }

        [NonSerialized]
        List<Glyph> glyphs;

        [NonSerialized]
        List<GlyphWord> words;

        [NonSerialized]
        List<GlyphBlock> blocks;

        [NonSerialized]
        List<UIVertex> cachedVertices;

        [NonSerialized]
        List<int> cachedIndices;

        bool useTypeComponent;

        public override Texture mainTexture
        {
            get
            {
                return Font.material.mainTexture;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            ForceRebuild();
        }

        protected override void Awake()
        {
            base.Awake();

            // using type component
            var typeComponent = GetComponent<GlyphAnimType>();
            useTypeComponent = typeComponent && typeComponent.enabled;

            ForceRebuild();
        }

		protected override void Start()
		{
			Font.textureRebuilt += OnFontTextureRebuilt;
		}

		protected override void OnDestroy()
		{
			Font.textureRebuilt -= OnFontTextureRebuilt;
		}

		// this is called when the font texture uvs change due to font size changes
		// so if this is bound to the same font we need to rebuild our vertices
		private void OnFontTextureRebuilt(Font changedFont)
		{
			if (changedFont != Font)
				return;
			ForceRebuild();
		}

#if UNITY_EDITOR
		public override void OnRebuildRequested()
        {
			Font.textureRebuilt -= OnFontTextureRebuilt;
			Font.textureRebuilt += OnFontTextureRebuilt;
			ForceRebuild();
        }
#endif

		public void CalculateLayoutInputHorizontal()
        {
            EnsureTextParsed();

            // figure out the  max width by laying out all on one line
            var temp = Layout(HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow);

            if (!AlignByGeometry)
                preferredWidth = temp[0].normalBounds.width;
            else
                preferredWidth = temp[0].pixelBounds.width;

            if (!Application.isPlaying)
                blocks = null;

            //Debug.Log("Calculated Layout preferred width=" + preferredWidth);

            minWidth = FontSize;
        }

        public void CalculateLayoutInputVertical()
        {
            EnsureTextParsed();

            // figure out the max height based on the current horizontal 
            // setting and the forced overflow vertical setting
            var temp = Layout(HorizontalOverflow, VerticalWrapMode.Overflow);

            if (!AlignByGeometry)
                preferredHeight = -temp[0].normalBounds.height;
            else
                preferredHeight = -temp[0].pixelBounds.height;

            if (!Application.isPlaying)
                blocks = null;

            //Debug.Log("Calculated Layout preferred height=" + preferredHeight);

            minHeight = fontLineHeight;
        }

        public void ForceRebuild()
        {
            // rebuild all glyphs and words
            EnsureTextParsed(true);

            // rebuild mesh
			SetVerticesDirty();
        }

        public void ForceBlocks()
        {
            blocks = null;
            SetVerticesDirty();
        }

        public void ForceLayout()
        {
            // this will force another layout when rebuild mesh
            for (var bi = 0; bi < blocks.Count; bi++)
            {
                blocks[bi].AdjustLineHorizontalOffset(this);
                blocks[bi].CalculatePosition(this);
            }
            positionIcons = true;
            cachedVertices = null;
            cachedIndices = null;

            // rebuild mesh
            SetVerticesDirty();
        }

        public void ForceNewPage()
        {
            // rebuild mesh
            cachedVertices = null;
            cachedIndices = null;
            positionIcons = true;
            PageNumber = Mathf.Clamp(PageNumber, 1, (blocks == null ? 0 : blocks.Count));
            RebindAnimatedGlyphs();
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // check for invalid layout
            if (blocks == null)
            {
                cachedVertices = null;
                cachedIndices = null;

                //Debug.Log("Creating blocks");
                blocks = Layout(HorizontalOverflow, VerticalOverflow);

                for (var bi = 0; bi < blocks.Count; bi++)
                    blocks[bi].CalculatePosition(this);
                positionIcons = true;

                PageNumber = Mathf.Clamp(PageNumber, 1, blocks.Count);
                GlyphAnim.insideGraphicRebuild = true;
                RebindAnimatedGlyphs();
                GlyphAnim.insideGraphicRebuild = false;
            }

            // pull out renderables
            var renderables = new List<ILTMeshRenderable>();
            if (HasBlocks)
            {
                var block = blocks[PageNumber - 1];
                for (var li = 0; li < block.Lines.Count; li++)
                {
                    var line = block.Lines[li];
                    for (var wi = 0; wi < line.Words.Count; wi++)
                    {
                        var word = line.Words[wi];
                        for (var gi = 0; gi < word.Glyphs.Count; gi++)
                        {
                            var glyph = word.Glyphs[gi] as ILTMeshRenderable;
                            if (glyph != null)
                                renderables.Add(glyph);
                        }
                    }
                }
            }

            // create caches
            if (cachedVertices == null)
            {
                //Debug.Log("Creating vertex cache");

                // vertices / indices
                cachedVertices = new List<UIVertex>(renderables.Count * 4);
                cachedIndices = new List<int>(renderables.Count * 6);
                for (var i = 0; i < cachedVertices.Capacity; i++)
                    cachedVertices.Add(UIVertex.simpleVert);
                for (var i = 0; i < cachedIndices.Capacity; i++)
                    cachedIndices.Add(0);
                for (var rendIndex = 0; rendIndex < renderables.Count; rendIndex++)
                {
                    var quadIndex = rendIndex * 4;
                    renderables[rendIndex].FillVertices(cachedVertices, quadIndex);
                    renderables[rendIndex].isDirty = false;

                    var indiIndex = rendIndex * 6;
                    cachedIndices[indiIndex + 0] = quadIndex + 0;
                    cachedIndices[indiIndex + 1] = quadIndex + 1;
                    cachedIndices[indiIndex + 2] = quadIndex + 2;
                    cachedIndices[indiIndex + 3] = quadIndex + 2;
                    cachedIndices[indiIndex + 4] = quadIndex + 3;
                    cachedIndices[indiIndex + 5] = quadIndex + 0;
                }

                // reposition icons 
                positionIcons = true;
            }
            // update cache
            else
            {
                // vertices
                for (var rendIndex = 0; rendIndex < renderables.Count; rendIndex++)
                    if (renderables[rendIndex].isDirty)
                    {
                        renderables[rendIndex].FillVertices(cachedVertices, rendIndex * 4);
                        renderables[rendIndex].isDirty = false;
                    }
            }

            // update the mesh
            vh.Clear();
            vh.AddUIVertexStream(cachedVertices, cachedIndices);
        }

        void EnsureTextParsed(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                if (!Font)
                {
                    Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    SetMaterialDirty();
                }
                glyphs = null;
                words = null;
                blocks = null;
            }
            if (glyphs == null)
            {
                // so these calculations are off...
                // ratio's are not exact
                fontLineHeight = (int)((float)Font.lineHeight * (float)(FontSize) / (float)Font.fontSize);
                fontLineAscent = (int)((float)Font.ascent * (float)(FontSize) / (float)Font.fontSize);
                fontLineHeight = (int)((float)fontLineHeight * LineSpacing);
                ParseGlyphs();
                ParseWords();
            }
        }

        void ParseGlyphs()
        {
            //Debug.Log("Creating glyphs");

            // create a default style
            var defaultStyle = new GlyphStyleDefault(
                FontStyle,
                FontSize,
                ColorOverride
                    ? new Color[] { ColorBL, ColorTL, ColorTR, ColorBR }
                    : new Color[] { Color, Color, Color, Color });

            // parse out the text 
            var parsedText = new List<GlyphTagParser.GlyphParsedChunk>();
            if (!RichText)
                parsedText.Add(new GlyphTagParser.GlyphParsedChunk() { style = defaultStyle, text = Text });
            else
                parsedText.AddRange(GlyphTagParser.Parse(ref Text, defaultStyle));

			// ensure all chunk characters are created in texture
			for (var ci = 0; ci < parsedText.Count; ci++)
				if (parsedText[ci].text.Length > 0)
					Font.RequestCharactersInTexture(parsedText[ci].text, parsedText[ci].style.getFontSize(), parsedText[ci].style.getFontStyle());

            // parse out all glyphs
            glyphs = new List<Glyph>();
            for (var ci = 0; ci < parsedText.Count; ci++)
            {
                // pull the chunk
                var chunk = parsedText[ci];

                // add all glyphs for the chunk
                if (chunk.text.Length > 0)
                {
                    for (var c = 0; c < chunk.text.Length; c++)
                    {
                        // create
                        switch (chunk.text[c])
                        {
                            case ' ': glyphs.Add(new SpaceGlyph(Font, chunk.style)); break;
                            case '\t': glyphs.Add(new TabGlyph(Font, chunk.style)); break;
                            case '\n': glyphs.Add(new ReturnGlyph(Font, chunk.style)); break;
                            case '\r': continue;
                            default: glyphs.Add(new CharGlyph(Font, chunk.text[c], chunk.style)); break;
                        }
                    }
                }
                else if (chunk.tag is GlyphTagIcon)
                {
                    // create
                    glyphs.Add(new IconGlyph(chunk.style));
                }
            }

            // debug out the parsing
            if (ShowDebugParse)
                foreach (var p in parsedText)
                    Debug.Log((p.tag == null ? string.Empty : p.tag.ToString()) + p.text);
        }

        void ParseWords()
        {
            // separate into words
            words = new List<GlyphWord>();
            var wordBuffer = new GlyphWord();
            for (int i = 0; i < glyphs.Count + 1; i++)
            {
                // build up buffer
                if (i < glyphs.Count && glyphs[i] is CharGlyph)
                    wordBuffer.AddGlyph(glyphs[i]);
                // commit
                else
                {
                    // commit buffered word
                    if (wordBuffer.Glyphs.Count > 0)
                    {
                        words.Add(wordBuffer);
                        wordBuffer = new GlyphWord();
                    }

                    // commit special char
                    if (i < glyphs.Count)
                    {
                        wordBuffer.AddGlyph(glyphs[i]);
                        words.Add(wordBuffer);
                        wordBuffer = new GlyphWord();
                    }
                }
            }
        }

        List<GlyphWord> BreakWord(GlyphWord word)
        {
            // can't break if there is only 1 glyph
            if (word.Glyphs.Count() == 1)
                return new List<GlyphWord>() { word };

            // break up this word into chunks
            var result = new List<GlyphWord>();
            var wordBuffer = new GlyphWord();
            while (word.Glyphs.Count > 0)
            {
                // add glyph to buffer
                wordBuffer.AddGlyph(word.Glyphs[0]);

                // check for overflow
                var wordInBounds = false;
                if (!AlignByGeometry)
                    wordInBounds = wordBuffer.normalizedWidth <= pixelPerfectBounds.width;
                else
                    wordInBounds = wordBuffer.pixelizedWidth <= pixelPerfectBounds.width;

                // handle breakage
                if (!wordInBounds)
                {
					// if there is only one glyph then everything is too small and can't continue
					if (wordBuffer.Glyphs.Count <= 1)
						return result;
					// continue breaking
                    wordBuffer.RemoveLastGlyph();
                    if (wordBuffer.Glyphs.Count > 0)
                        result.Add(wordBuffer);
                    wordBuffer = new GlyphWord();
                    wordBuffer.AddGlyph(word.Glyphs[0]);
                }

                word.Glyphs.RemoveAt(0);
            }

            // commit last buffer
            if (wordBuffer.Glyphs.Count > 0)
                result.Add(wordBuffer);

            // complete
            return result;
        }

        List<GlyphBlock> Layout(HorizontalWrapMode horizontalWrapMode, VerticalWrapMode verticalWrapMode)
        {
            pixelPerfectBounds = GetPixelAdjustedRect();
            var result = new List<GlyphBlock>();

            // separate into lines
            var blockBuffer = new GlyphBlock();
            var lineBuffer = new GlyphLine();
            for (var i = 0; i < words.Count; i++)
            {
                // add the word to the line
                lineBuffer.AddWord(words[i], this);

                // check for out of bounds
                var wordInBounds = false;
                if (!AlignByGeometry)
                    wordInBounds = lineBuffer.normalBounds.width <= pixelPerfectBounds.width;
                else
                    wordInBounds = lineBuffer.pixelBounds.width <= pixelPerfectBounds.width;

                // word is always in bounds if there is no wrapping taking place
                if (!wordInBounds && horizontalWrapMode == HorizontalWrapMode.Overflow)
                    wordInBounds = true;

                // break word up if out of bounds and the only word on the line
                if (!wordInBounds && lineBuffer.Words.Count == 1)
                {
                    var wordChunks = BreakWord(words[i]);
					// if there are no word chunks then everything is way too small and we are finished
					if (wordChunks.Count == 0)
						return result;
                    words.RemoveAt(i);
                    words.InsertRange(i, wordChunks);
                    lineBuffer.RemoveLastWord(this);
                    lineBuffer.AddWord(wordChunks[0], this);
                    wordInBounds = true;
                }

                // determine if we break to new line
                var newLine = !wordInBounds;

                // negate the new line if it is the first whitespace after a word
                if (newLine && words[i].IsWhiteSpace && i - 1 >= 0 && !words[i - 1].IsWhiteSpace)
                    newLine = false;

                // force a new line if the word is a new line glyph
                if (!newLine && words[i].IsReturn)
                    newLine = true;

                // commit line 
                if (newLine)
                {
                    // take out the last word that broke the line
                    // unless it is a single return
                    if (lineBuffer.Words.Count > 1)
                        lineBuffer.RemoveLastWord(this);
                    // remove any whitespace at the end of the line
                    if (lineBuffer.Words.Last().IsWhiteSpace)
                        lineBuffer.RemoveLastWord(this);
                    // add the line
                    blockBuffer.AddLine(lineBuffer, this);

                    // check to see if the block is past the bounds for pagination
                    var blockInBounds = false;
                    if (!AlignByGeometry)
                        blockInBounds = -blockBuffer.normalBounds.height <= pixelPerfectBounds.height;
                    else
                        blockInBounds = -blockBuffer.pixelBounds.height <= pixelPerfectBounds.height;

                    // always in bounds if no pagination
                    if (!blockInBounds && verticalWrapMode == VerticalWrapMode.Overflow)
                        blockInBounds = true;

                    // commit the block prior to the breakage
                    if (!blockInBounds)
                    {
                        // remove the last line
                        blockBuffer.RemoveLastLine(this);

						// if the block buffer doesn't have any lines we can't move on
						if (blockBuffer.Lines.Count == 0)
							return result;

                        // handle page break truncation
                        var pageBreakingWord = FindPageBreakInBlock(blockBuffer);
                        if (pageBreakingWord != null)
                            TruncateBlockAtWord(blockBuffer, pageBreakingWord);

                        // append block
                        result.Add(blockBuffer);

						// we are finished if there are no last lines or last words
						if (blockBuffer.Lines.LastOrDefault() == null ||
							 blockBuffer.Lines.Last().Words.LastOrDefault() == null)
							return result;

                        // move to just after the last added word in the block
                        i = words.IndexOf(blockBuffer.Lines.Last().Words.Last()) + 1;
                        blockBuffer = new GlyphBlock();
                    }

                    // create new
                    lineBuffer = new GlyphLine();

                    // reprocess the word that broke the line 
                    // unless it was a forced line break
                    if (!words[i].IsReturn)
                        i--;

                }

                // commit final working buffer 
                if (i == words.Count - 1)
                {
                    if (lineBuffer.Words.Count > 0)
                        blockBuffer.AddLine(lineBuffer, this);

                    // append last 
                    if (blockBuffer.Lines.Count > 0)
                        result.Add(blockBuffer);
                }

            }

            // return
            return result;
        }

        GlyphWord FindPageBreakInBlock(GlyphBlock block)
        {
            // no page break
            if (PageBreakChars.Count == 0)
                return null;

            // look from end
            for (var li = block.Lines.Count - 1; li >= 0; li--)
            {
                var line = block.Lines[li];
                for (var wi = line.Words.Count - 1; wi >= 0; wi--)
                {
                    var lastGlyph = line.Words[wi].Glyphs.Last() as CharGlyph;
                    if (lastGlyph != null && PageBreakChars.Contains(lastGlyph.id))
                        return line.Words[wi];
                }
            }

            // nothing found
            return null;
        }

        void TruncateBlockAtWord(GlyphBlock block, GlyphWord word)
        {
            while (true)
            {
                var currentLine = block.Lines.Last();
                while (true)
                {
                    // completion check
                    if (currentLine.Words.Last() == word)
                        return;

                    // progress backwards on line
                    currentLine.RemoveLastWord(this);

                    // check for end of line
                    if (currentLine.Words.Count == 0)
                        break;
                }
                // progress backwards on block
                block.RemoveLastLine(this);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 1, 0.25f);
            var canvasPosition = Vector3.zero;

            if (HasBlocks)
            {
                var block = blocks[PageNumber - 1];

                if (ShowDebugBlock)
                {
                    if (!AlignByGeometry)
                        DrawWireCubeGizmoOnCanvas(block.position, block.normalBounds);
                    else
                        DrawWireCubeGizmoOnCanvas(block.position, block.pixelBounds);
                }

                for (var li = 0; li < block.Lines.Count; li++)
                {
                    var line = block.Lines[li];

                    if (ShowDebugLines)
                    {
                        if (!AlignByGeometry)
                            DrawWireCubeGizmoOnCanvas(line.position, line.normalBounds);
                        else
                            DrawWireCubeGizmoOnCanvas(line.position, line.pixelBounds);
                    }

                    for (var wi = 0; wi < line.Words.Count; wi++)
                    {
                        var word = line.Words[wi];

                        if (ShowDebugWords)
                            DrawWireCubeGizmoOnCanvas(word.position, word.pixelBounds);

                        if (ShowDebugGlyphs)
                            for (var gi = 0; gi < word.Glyphs.Count; gi++)
                                DrawWireCubeGizmoOnCanvas(word.Glyphs[gi].position, word.Glyphs[gi].pixelBounds);
                    }
                }
            }
        }

        void DrawWireCubeGizmoOnCanvas(Vector2 position, Rect bounds)
        {
            // bottom left -> top right
            Vector3 bl = position + bounds.min; bl.z = 0;
            Vector3 tr = position + bounds.max; tr.z = 0;

            // transform from canvas to world
            bl = rectTransform.localToWorldMatrix.MultiplyPoint(bl);
            tr = rectTransform.localToWorldMatrix.MultiplyPoint(tr);

            // calculate center and radius
            var center = bl + (tr - bl) / 2;
            var radius = tr - bl;

            Gizmos.DrawWireCube(center, radius);
        }

        void DrawWireCubeGizmoOnCanvas(Rect bounds)
        {
            // bottom left -> top right
            Vector3 bl = bounds.min; bl.z = 0;
            Vector3 tr = bounds.max; tr.z = 0;

            // transform from canvas to world
            bl = rectTransform.localToWorldMatrix.MultiplyPoint(bl);
            tr = rectTransform.localToWorldMatrix.MultiplyPoint(tr);

            // calculate center and radius
            var center = bl + (tr - bl) / 2;
            var radius = tr - bl;

            Gizmos.DrawWireCube(center, radius);
        }

        bool positionIcons;
        void Update()
        {
            if (blocks == null)
                return;

            // only place viable to make sure icon components are instantiated
            if (positionIcons)
            {
                CleanupIcons();
                positionIcons = RenderIcons();
            }

        }

        void CleanupIcons()
        {
            var allChildImageComponents = GetComponentsInChildren<Image>(true);
            var validChildImageComponents = new List<Image>();

            // go through all paged glyphs
            if (HasBlocks)
            {
                var block = blocks[PageNumber - 1];
                for (var li = 0; li < block.Lines.Count; li++)
                {
                    var line = block.Lines[li];
                    for (var wi = 0; wi < line.Words.Count; wi++)
                    {
                        var word = line.Words[wi];
                        for (var gi = 0; gi < word.Glyphs.Count; gi++)
                        {
                            var glyph = word.Glyphs[gi] as IconGlyph;
                            if (glyph != null)
                                validChildImageComponents.Add(glyph.imageComponent);
                        }
                    }
                }
            }

            // remove components no longer referenced
            allChildImageComponents
                .Where(d => !validChildImageComponents.Contains(d))
                .ToList()
                .ForEach(d => DestroyImmediate(d.gameObject));
        }

        bool RenderIcons()
        {
            var changeOccurred = false;
            if (HasBlocks)
            {
                var block = blocks[PageNumber - 1];
                for (var li = 0; li < block.Lines.Count; li++)
                {
                    var line = block.Lines[li];
                    for (var wi = 0; wi < line.Words.Count; wi++)
                    {
                        var word = line.Words[wi];
                        for (var gi = 0; gi < word.Glyphs.Count; gi++)
                        {
                            var glyph = word.Glyphs[gi] as IconGlyph;
                            if (glyph != null)
                            {
                                var prior = glyph.imageComponent != null
                                    ? glyph.imageComponent.rectTransform.localPosition
                                    : Vector3.zero;
                                glyph.Render(this);
                                if (prior != glyph.imageComponent.rectTransform.localPosition)
                                    changeOccurred = true;
                            }
                        }
                    }
                }
            }
            return changeOccurred;
        }

        void RebindAnimatedGlyphs()
        {
            if (!Application.isPlaying)
                return;

            if (useTypeComponent)
                GetComponent<GlyphAnimType>().enabled = false;

            // remove all glyph animations
            var animations = GetComponents<GlyphAnim>().ToList();
            for (var ai = 0; ai < animations.Count; ai++)
                animations[ai].ClearGlyphs();

            // go through all glyphs in current page
            if (HasBlocks)
            {
                var block = blocks[PageNumber - 1];
                for (var li = 0; li < block.Lines.Count; li++)
                {
                    var line = block.Lines[li];
                    for (var wi = 0; wi < line.Words.Count; wi++)
                    {
                        var word = line.Words[wi];
                        for (var gi = 0; gi < word.Glyphs.Count; gi++)
                        {
                            var glyph = word.Glyphs[gi];

                            // determine what animations are a part of this chunk if any
                            var supportedAnimations = animations.Where(d => glyph.style.hasAnim(d.tagName)).ToList();

                            // bind the glyph
                            for (var a = 0; a < supportedAnimations.Count; a++)
                                supportedAnimations[a].AddGlyph(glyph);
                        }
                    }
                }
            }

            if (useTypeComponent)
                GetComponent<GlyphAnimType>().enabled = true;
        }
    }
}