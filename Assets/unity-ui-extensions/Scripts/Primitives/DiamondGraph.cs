/// Credit koohddang
/// Sourced from - http://forum.unity3d.com/threads/onfillvbo-to-onpopulatemesh-help.353977/#post-2299311

using System;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
    public class DiamondGraph : UIPrimitiveBase
    {
        [SerializeField]
        public float[] points = new float[8];

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            float wHalf = rectTransform.rect.width / 2;
            //float hHalf = rectTransform.rect.height / 2;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Math.Min(1, Math.Max(0, points[i]));
            }

            float angleOffset = (Mathf.PI*2f) / points.Length;
            Color32 color32 = color;
            vh.AddVert(Vector3.zero, color32, Vector2.one * 0.5f);
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 normalizedPosition = new Vector3(Mathf.Sin(angleOffset * i),
                                                         Mathf.Cos(angleOffset * i), 0);
                vh.AddVert(normalizedPosition * (- wHalf * points[i]), color32, ((Vector2.one*0.5f)-(Vector2)normalizedPosition * 0.5f));
            }

            for (int i = 0; i < points.Length; i++)
            {
                vh.AddTriangle(0, i+1, (i + 2 >= vh.currentVertCount ? 1 : i+2 ) );
            }
        }
    }
}