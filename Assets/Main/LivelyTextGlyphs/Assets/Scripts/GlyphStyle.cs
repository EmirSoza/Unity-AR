using UnityEngine;

namespace LivelyTextGlyphs
{
    public abstract class GlyphStyle
    {
        public GlyphStyle parent { get; set; }

        // recurse down until the root implements a valid value
        public virtual FontStyle getFontStyle()
        {
            return parent.getFontStyle();
        }

        // recurse down until the root implements a valid value
        public virtual int getFontSize()
        {
            return parent.getFontSize();
        }

        // recurse down until the root implements a valid value
        public virtual Color[] getVertexColors()
        {
            return parent.getVertexColors();
        }

        // recurse down until the root implements a valid value
        public virtual string getName()
        {
            return parent.getName();
        }

        // return the root
        public GlyphStyle defaultStyle
        {
            get
            {
                var node = this;
                while (node.parent != null)
                    node = node.parent;
                return node;
            }
        }

        // return if any styles in the chain contain the requested style
        protected bool hasStyle(FontStyle style)
        {
            var node = parent;
            while (node != null)
            {
                if (node.getFontStyle() == style)
                    return true;
                node = node.parent;
            }
            return false;
        }

        public bool hasAnim(string name)
        {
            var node = this;
            do
            {
                if (node is GlyphStyleAnim && (node as GlyphStyleAnim).getName() == name)
                    return true;
                node = node.parent;
            } while (node != null);
            return false;
        }

        public override string ToString()
        {
            return parent != null
                ? parent + "." + GetType().Name
                : GetType().Name;
        }

    }

    public class GlyphStyleDefault : GlyphStyle
    {
        FontStyle defaultFontStyle;
        int defaultFontSize;
        Color[] defaultFontVertexColors;

        public GlyphStyleDefault(FontStyle fontStyle, int fontSize, Color[] fontVertexColors)
        {
            defaultFontStyle = fontStyle;
            defaultFontSize = fontSize;
            defaultFontVertexColors = fontVertexColors;
        }

        public override FontStyle getFontStyle()
        {
            return defaultFontStyle;
        }

        public override int getFontSize()
        {
            return defaultFontSize;
        }

        public override Color[] getVertexColors()
        {
            return defaultFontVertexColors;
        }

        public override string getName()
        {
            return string.Empty;
        }
    }

    public class GlyphStyleBold : GlyphStyle
    {
        public override FontStyle getFontStyle()
        {
            return hasStyle(FontStyle.BoldAndItalic) || hasStyle(FontStyle.Italic) ? FontStyle.BoldAndItalic : FontStyle.Bold;
        }
    }

    public class GlyphStyleItalic : GlyphStyle
    {
        public override FontStyle getFontStyle()
        {
            return hasStyle(FontStyle.BoldAndItalic) || hasStyle(FontStyle.Bold) ? FontStyle.BoldAndItalic : FontStyle.Italic;
        }
    }

    public class GlyphStyleSize : GlyphStyle
    {
        float percentageOverride;

        public GlyphStyleSize(float percentage)
        {
            percentageOverride = percentage;
        }

        public override int getFontSize()
        {
            return (int)((float)defaultStyle.getFontSize() * percentageOverride);
        }
    }

    public class GlyphStyleColor : GlyphStyle
    {
        Color[] vertexColorsOverride;

        public GlyphStyleColor(Color[] vertexColors)
        {
            vertexColorsOverride = vertexColors;
        }

        public override Color[] getVertexColors()
        {
            return vertexColorsOverride;
        }
    }

    public class GlyphStyleIcon : GlyphStyle
    {
        string nameOverride;

        public GlyphStyleIcon(string name)
        {
            nameOverride = name;
        }

        public override string getName()
        {
            return nameOverride;
        }
    }

    public class GlyphStyleAnim : GlyphStyle
    {
        string nameOverride;

        public GlyphStyleAnim(string name)
        {
            nameOverride = name;
        }

        public override string getName()
        {
            return nameOverride;
        }
    }
}