using System.Collections.Generic;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public interface ILTMeshRenderable
    {
        bool isDirty { get; set; }
        void FillVertices(List<UIVertex> stream, int index);
    }

    public interface ILTIconRenderable
    {
        void Render(LTText parent);
    }
}
