using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LivelyTextGlyphs
{
    public abstract class Glyph
    {
        // style assigned
        public GlyphStyle style { get; private set; }

        // distance between this glyph and the next
        public int advance { get; protected set; }

        // bottom left -> top right bounds 
        public Rect pixelBounds { get; protected set; }

        // local horizontal position
        public int localX { get; set; }

        // bottom left position on the canvas
        public Vector2 position { get; private set; }

        // offset from position on the canvas
        public virtual Vector3 offset { get; set; }

        // visibility
        public virtual bool visible { get; set; }

        // opacity
        public virtual float opacity { get; set; }

        // hue
        public virtual Color hue { get; set; }

        public Glyph(GlyphStyle style)
        {
            this.style = style;
            this.visible = true;
            this.opacity = 1.0f;
            this.hue = new Color(1, 1, 1, 0);
        }

        public float normalizedWidth { get { return advance; } }
        public float pixelizedWidth { get { return pixelBounds.width; } }

        public virtual void CalculatePosition(Vector2 parentPosition)
        {
            position =
                parentPosition +
                new Vector2(localX, 0);
        }
    }

    public class CharGlyph : Glyph, ILTMeshRenderable
    {
        // character identifier 'a' 'b' etc
        public char id { get; set; }

        Vector2[] uvs;

        public CharGlyph(Font font, char id, GlyphStyle style) : base(style)
        {
            this.id = id;

            CharacterInfo info;
            font.GetCharacterInfo(id, out info, style.getFontSize(), style.getFontStyle());

            advance = info.advance;

            pixelBounds = new Rect()
            {
                xMin = info.minX,
                yMin = info.minY,
                xMax = info.maxX,
                yMax = info.maxY
            };

            uvs = new Vector2[4];
            uvs[0] = info.uvBottomLeft;
            uvs[1] = info.uvTopLeft;
            uvs[2] = info.uvTopRight;
            uvs[3] = info.uvBottomRight;
        }

        // string representation
        public override string ToString()
        {
            return id.ToString();
        }

        bool _isDirty;
        public bool isDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public override Vector3 offset
        {
            get { return base.offset; }
            set
            {
                base.offset = value;
                isDirty = true;
            }
        }

        public override bool visible
        {
            get { return base.visible; }
            set
            {
                base.visible = value;
                isDirty = true;
            }
        }

        public override float opacity
        {
            get { return base.opacity; }
            set
            {
                base.opacity = Mathf.Clamp01(value);
                isDirty = true;
            }
        }

        public override Color hue
        {
            get { return base.hue; }
            set
            {
                base.hue = value;
                isDirty = true;
            }
        }

        public override void CalculatePosition(Vector2 parentPosition)
        {
            // normal calculation
            base.CalculatePosition(parentPosition);
            isDirty = true;
        }

        public void FillVertices(List<UIVertex> stream, int index)
        {
            // bottom left position after offset
            var origin = new Vector3(position.x, position.y) + offset;

            // default color to our style (perform a copy)
            var colors = style.getVertexColors().Select(c => new Color(c.r, c.g, c.b, c.a)).ToArray();

            // lerp style color into hue color by hue alpha
            for (int i = 0; i < 4; i++)
            {
                if (hue.a > 0)
                {
                    colors[i].r = Mathf.Lerp(colors[i].r, hue.r, hue.a);
                    colors[i].g = Mathf.Lerp(colors[i].g, hue.g, hue.a);
                    colors[i].b = Mathf.Lerp(colors[i].b, hue.b, hue.a);
                }
                colors[i].a *= opacity;
            }

            // bottom left
            stream[index + 0] = new UIVertex()
            {
                position = origin + (visible ? new Vector3(pixelBounds.xMin, pixelBounds.yMin) : Vector3.zero),
                normal = Vector3.forward,
                color = colors[0],
                uv0 = uvs[0],
            };

            // top left
            stream[index + 1] = new UIVertex()
            {
                position = origin + (visible ? new Vector3(pixelBounds.xMin, pixelBounds.yMax) : Vector3.zero),
                normal = Vector3.forward,
                color = colors[1],
                uv0 = uvs[1],
            };

            // top right
            stream[index + 2] = new UIVertex()
            {
                position = origin + (visible ? new Vector3(pixelBounds.xMax, pixelBounds.yMax) : Vector3.zero),
                normal = Vector3.forward,
                color = colors[2],
                uv0 = uvs[2],
            };

            // bottom right
            stream[index + 3] = new UIVertex()
            {
                position = origin + (visible ? new Vector3(pixelBounds.xMax, pixelBounds.yMin) : Vector3.zero),
                normal = Vector3.forward,
                color = colors[3],
                uv0 = uvs[3],
            };
        }
    }

    public class SpaceGlyph : Glyph
    {
        public SpaceGlyph(Font font, GlyphStyle style) : base(style)
        {
            CharacterInfo info;

            font.GetCharacterInfo(' ', out info, style.getFontSize(), style.getFontStyle());

            advance = info.advance;

            pixelBounds = new Rect()
            {
                xMin = info.minX,
                yMin = info.minY,
                xMax = info.maxX,
                yMax = info.maxY
            };
        }

        // string representation
        public override string ToString()
        {
            return "|s|";
        }
    }

    public class TabGlyph : Glyph
    {
        public TabGlyph(Font font, GlyphStyle style) : base(style)
        {
            CharacterInfo info;
            font.GetCharacterInfo(' ', out info, style.getFontSize(), style.getFontStyle());

            advance = info.advance * 5;

            pixelBounds = new Rect()
            {
                xMin = info.minX,
                yMin = info.minY,
                xMax = info.maxX + (info.maxX - info.minX) * 5,
                yMax = info.maxY
            };
        }

        // string representation
        public override string ToString()
        {
            return "|t|";
        }
    }

    public class ReturnGlyph : Glyph
    {
        public ReturnGlyph(Font font, GlyphStyle style) : base(style)
        {
            CharacterInfo info;
            font.GetCharacterInfo('X', out info, style.getFontSize(), style.getFontStyle());

            advance = 0;

            pixelBounds = new Rect()
            {
                xMin = 0,
                yMin = info.minY,
                xMax = 0,
                yMax = info.maxY
            };
        }

        // string representation
        public override string ToString()
        {
            return "|n|";
        }
    }

    public class IconGlyph : Glyph, ILTIconRenderable
    {
        public string iconName;
        public Image imageComponent;

        public IconGlyph(GlyphStyle style) : base(style)
        {
            advance = style.defaultStyle.getFontSize();

            var offset = advance / 4f;

            pixelBounds = new Rect()
            {
                xMin = 0,
                yMin = 0 - offset,
                xMax = advance,
                yMax = advance - offset
            };

            iconName = style.getName();
        }

        // string representation
        public override string ToString()
        {
            return "|i|";
        }

        public override Vector3 offset
        {
            get { return base.offset; }
            set
            {
                base.offset = value;
                if (imageComponent)
                {
                    imageComponent.rectTransform.localPosition = new Vector3(
                        position.x + value.x + pixelBounds.xMin,
                        position.y + value.y + pixelBounds.yMin,
                        offset.z);
                }
            }
        }

        public override bool visible
        {
            get { return base.visible; }
            set
            {
                base.visible = value;
                if (imageComponent && !GlyphAnim.insideGraphicRebuild)
                    imageComponent.enabled = visible;
            }
        }

        public override float opacity
        {
            get { return base.opacity; }
            set
            {
                base.opacity = Mathf.Clamp01(value);
                if (imageComponent)
                    imageComponent.color = new Color(1, 1, 1, opacity);
            }
        }

        public void Render(LTText parent)
        {
            // ensure component is created
            var createNew = !imageComponent;
            if (createNew)
            {
                imageComponent = new GameObject("icon" + (!string.IsNullOrEmpty(iconName) ? "_" + iconName : string.Empty), typeof(Image)).GetComponent<Image>();
                imageComponent.rectTransform.anchorMin = imageComponent.rectTransform.anchorMax = Vector2.zero;
                imageComponent.rectTransform.pivot = Vector2.zero;
                imageComponent.rectTransform.localScale = Vector3.one;
                imageComponent.rectTransform.localRotation = Quaternion.identity;
                imageComponent.sprite = parent.IconAtlas ? parent.IconAtlas.GetSprite(iconName) : null;
                imageComponent.gameObject.hideFlags = HideFlags.DontSaveInEditor;
                imageComponent.hideFlags = HideFlags.DontSaveInEditor;
                imageComponent.enabled = visible;
            }

            // update positioning
            imageComponent.rectTransform.sizeDelta = pixelBounds.size;
            imageComponent.rectTransform.localPosition = new Vector3(
                position.x + offset.x + pixelBounds.xMin,
                position.y + offset.y + pixelBounds.yMin,
                offset.z);

            // ensure component is parented
            if (createNew)
                imageComponent.gameObject.transform.SetParent(parent.transform, false);
        }
    }
}