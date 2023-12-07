using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;

public static class LayerTextures
{ 
    public struct LayerParams
    {
        public Vector2 size;
        public Vector2 offset;
        public Texture2D texture;
        public Color color;
    }


    public static Sprite GetLayeredTexture (List<LayerParams> layers, Vector2Int size)
    {
        Texture2D returnTexture = new Texture2D(size.x, size.y);

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                returnTexture.SetPixel(x, y, Color.clear);

        for (int layer = 0; layer < layers.Count; layer++)
        {
            ApplyLayer(returnTexture, layers[layer]);
        }
        returnTexture.Apply();

        return Sprite.Create( returnTexture, new Rect(0,0, size.x, size.y), new Vector2(0.5f, 0.5f) );
    }

    private static void ApplyLayer(Texture2D returnTexture, LayerParams layerParams)
    {
        if (layerParams.size == default)
        {
            layerParams.size.x = layerParams.texture.width;
            layerParams.size.y = layerParams.texture.height;
        }

        Vector2Int offset = new Vector2Int();
        if (layerParams.offset != default)
        {
            offset.x = Mathf.RoundToInt(returnTexture.width * layerParams.offset.x);
            offset.y = Mathf.RoundToInt(returnTexture.height * layerParams.offset.y);
        }

        Texture2D resizedTexture = GetResizedTexture(ref layerParams, returnTexture.height, returnTexture.width);

        for (int x = 0; x < resizedTexture.width; x++)
        {
            for (int y = 0; y < resizedTexture.height; y++)
            {
                Vector2Int transformedPosition = offset;
                transformedPosition.x += x;
                transformedPosition.y += y;

                Color previousColor = returnTexture.GetPixel(transformedPosition.x, transformedPosition.y);

                Color newColor;
                if (layerParams.color != default)
                    newColor = GetNewColor(resizedTexture, new Vector2Int(x, y), layerParams.color);
                else
                    newColor = GetNewColor(resizedTexture, new Vector2Int(x, y));

                float newAlpha = newColor.a + (newColor.a < 1 ? (1 - newColor.a) * previousColor.a : 0);

                Color pixelColor = Color.Lerp(previousColor, newColor, newColor.a);
                pixelColor.a = newAlpha;
                returnTexture.SetPixel(transformedPosition.x, transformedPosition.y, pixelColor);
            }
        }
    }

    private static bool IsResizingRequired(LayerParams layer)
    {
        return layer.size != default
               && 
                   layer.texture.height != layer.size.y
                   ||
                   layer.texture.width != layer.size.x;
    }

    private static Texture2D GetResizedTexture(ref LayerParams layerParams, int height, int width)
    {
        Texture2D resizedTexture = layerParams.texture;

        if (IsResizingRequired(layerParams))
        {
            Vector2Int newSize = new Vector2Int();
            newSize.x = Mathf.RoundToInt(width * layerParams.size.x);
            newSize.y = Mathf.RoundToInt(height * layerParams.size.y);

            resizedTexture = new Texture2D(layerParams.texture.width, layerParams.texture.height, TextureFormat.RGBA32,false);
            Color[] pixels = layerParams.texture.GetPixels();
            resizedTexture.SetPixels(pixels);
            
            TextureScale.Bilinear(resizedTexture, newSize.x, newSize.y);
        }
        return resizedTexture;
    }

    private static Texture2D GetResizedTexture(Texture2D texture, Vector2Int size)
    {
        Texture2D resizedTexture;

        resizedTexture = new Texture2D(0, 0);
        Graphics.CopyTexture(texture, resizedTexture);

        TextureScale.Bilinear(resizedTexture, size.x, size.y);

        return resizedTexture;
    }

    private static Color GetNewColor(Texture2D newLayer, Vector2Int position)
    {
        return newLayer.GetPixel(position.x, position.y);
    }

    private static Color GetNewColor(Texture2D newLayer, Vector2Int position, Color tintColor)
    {
        float colorValue = newLayer.GetPixel(position.x, position.y).grayscale;
        return new Color(colorValue * tintColor.r, colorValue * tintColor.g, colorValue * tintColor.b, newLayer.GetPixel(position.x, position.y).a);
    }
}
