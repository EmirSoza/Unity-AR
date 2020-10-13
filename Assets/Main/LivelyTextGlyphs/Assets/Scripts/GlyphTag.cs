using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public abstract class GlyphTag
    {
        public override bool Equals(object obj)
        {
            if (obj is GlyphTag && (obj as GlyphTag) == this)
                return true;
            else if (obj is string && (obj as string) == getIdentifier())
                return true;
            return false;
        }
        protected abstract string getIdentifier();

        public override int GetHashCode()
        {
            return getIdentifier().GetHashCode();
        }

        public GlyphStyle Instanciate(string[] parms)
        {
            return instanciate(parms);
        }
        protected abstract GlyphStyle instanciate(string[] parms);

        public Type StyleType { get { return getStyleType(); } }
        protected abstract Type getStyleType();

        public override string ToString()
        {
            return "<" + getIdentifier() + ">";
        }

    }

    public class GlyphTagBold : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "b";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleBold);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            return new GlyphStyleBold();
        }
    }

    public class GlyphTagItalic : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "i";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleItalic);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            return new GlyphStyleItalic();
        }
    }

    public class GlyphTagSize : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "size";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleSize);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            float val = 0;
            if (parms.Length > 0)
                if (!float.TryParse(parms[0], out val))
                    val = 1.0f;
            return new GlyphStyleSize(val);
        }
    }

    public class GlyphTagColor : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "color";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleColor);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            var bl = Color.magenta; var tl = Color.magenta; var tr = Color.magenta; var br = Color.magenta;
            if (parms.Length > 0 && !ColorUtility.TryParseHtmlString(parms[0], out bl))
                bl = Color.magenta;
            if (parms.Length > 1 && !ColorUtility.TryParseHtmlString(parms[1], out tl))
                tl = Color.magenta;
            if (parms.Length > 2 && !ColorUtility.TryParseHtmlString(parms[2], out tr))
                tr = Color.magenta;
            if (parms.Length > 3 && !ColorUtility.TryParseHtmlString(parms[3], out br))
                br = Color.magenta;

            //  if there was only 1 color then override the other 3
            if (parms.Length == 1)
                tl = tr = br = bl;
            // if there were 2 parameters then override the right side with the left
            else if (parms.Length == 2)
            {
                br = bl;
                tr = tl;
            }
            // if there were 3 parameters then override the bottom right with the bottom left
            else if (parms.Length == 3)
            {
                br = bl;
            }
            return new GlyphStyleColor(new Color[] { bl, tl, tr, br });
        }
    }

    public class GlyphTagIcon : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "icon";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleIcon);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            return new GlyphStyleIcon(parms.Length > 0 ? parms[0] : string.Empty);
        }
    }

    public class GlyphTagAnim : GlyphTag
    {
        protected override string getIdentifier()
        {
            return "anim";
        }

        protected override Type getStyleType()
        {
            return typeof(GlyphStyleAnim);
        }

        protected override GlyphStyle instanciate(string[] parms)
        {
            return new GlyphStyleAnim(parms.Length > 0 ? parms[0] : string.Empty);
        }
    }

    public class GlyphTagParser
    {
        // expression to pull out all contents between <***>
        const string HtmlTagExpression = @"<(.*?)>";

        // structure to hold parsed chunks with a given style assigned
        public class GlyphParsedChunk
        {
            public GlyphTag tag { get; set; }
            public GlyphStyle style { get; set; }
            public string text { get; set; }
        }

        public static List<GlyphParsedChunk> Parse(ref string text, GlyphStyle defaultStyle)
        {
            var currentChunk = new GlyphParsedChunk() { style = defaultStyle };
            var result = new List<GlyphParsedChunk>() { currentChunk };
            var lastStyle = currentChunk.style;

            var htmlTags = Regex.Matches(text, HtmlTagExpression).OfType<Match>().ToList();

            // handle no matches
            if (htmlTags.Count == 0)
            {
                currentChunk.text = text;
                return result;
            }

            // setup list of supported tags
            var supportedTags = new List<GlyphTag>()
        {
            new GlyphTagBold(),
            new GlyphTagItalic(),
            new GlyphTagSize(),
            new GlyphTagColor(),
            new GlyphTagIcon(),
            new GlyphTagAnim()
        };

            // go through all matched tags
            for (var i = 0; i < htmlTags.Count; i++)
            {
                // here marks the start of a new chunk

                // before creating a new one we need to append text to the current chunk up until this point
                currentChunk.text = i == 0
                    ? text.SubstringUpToMatch(htmlTags[i])
                    : text.SubstringBetweenMatches(htmlTags[i - 1], htmlTags[i]);

                // create new chunk
                currentChunk = new GlyphParsedChunk();
                result.Add(currentChunk);

                // now based on the tag we need to do one of the following
                // a) keep the existing style if we don't find a supported tag
                // b) create a new style if we find a supporting tag
                // c) remove the current style if it is the end of the current style </TAG>

                var tagName = string.Empty;
                var tagParams = new string[] { };
                var tagIsClosing = false;
                ParseHtmlTag(htmlTags[i].Groups[1].Value, out tagName, out tagParams, out tagIsClosing);

                var supportedTag = supportedTags.FirstOrDefault(d => d.Equals(tagName));

                // a) keep existing style
                if (!tagIsClosing && supportedTag == null)
                {
                    currentChunk.style = lastStyle;
                }
                // b) create new style
                else if (!tagIsClosing && supportedTag != null)
                {
                    currentChunk.tag = supportedTag;
                    currentChunk.style = supportedTag.Instanciate(tagParams);
                    currentChunk.style.parent = lastStyle;

                    // if icon then we auto close and reopen a new chunk
                    if (supportedTag.StyleType == typeof(GlyphStyleIcon))
                    {
                        currentChunk.text = string.Empty;
                        currentChunk = new GlyphParsedChunk();
                        currentChunk.style = lastStyle;
                        result.Add(currentChunk);
                    }
                }
                // c) fall back a style
                else if (tagIsClosing && supportedTag != null && lastStyle.GetType() == supportedTag.StyleType)
                {
                    currentChunk.style = lastStyle.parent;
                }
                // some other unexpected error so keep existing style
                else
                {
                    currentChunk.style = lastStyle;
                }

                // keep our last style in check
                lastStyle = currentChunk.style;
            }

            // append the last remaining text to our last chunk
            currentChunk.text = text.SubstringAfterMatch(htmlTags.Last());

            // complete
            return result;
        }

        static void ParseHtmlTag(string content, out string name, out string[] parms, out bool isClosing)
        {
            name = string.Empty;
            parms = new string[] { };
            isClosing = content.Length > 0 && content[0] == '/';

            var equalIndex = content.IndexOf('=');

            // parse name only
            if (equalIndex < 0)
                name = isClosing
                    ? content.Substring(1).Trim()
                    : content.Trim();

            // parse name and parameters
            else
            {
                name = isClosing
                    ? content.Substring(1, equalIndex - 1)
                    : content.Substring(0, equalIndex - 0);

                parms = content.Substring(equalIndex + 1).Split(',');
                for (var i = 0; i < parms.Length; i++)
                    parms[i] = parms[i].Trim();
            }
        }
    }

   
}