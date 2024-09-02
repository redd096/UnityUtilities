using System.Collections.Generic;
using UnityEngine;

namespace redd096.IconGenerator
{
    public static class IconHelper
    {
        /// <summary>
        /// Combine texture with overlays
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="overlayIcons"></param>
        /// <returns></returns>
        public static Texture2D ApplyOverlaysToTexture(Texture2D texture, List<Texture2D> overlayIcons)
        {
            //if there aren't overlays, return texture
            if (overlayIcons == null || overlayIcons.Count == 0)
                return texture;

            //else combine with overlays
            Texture2D finalIcon = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            for (int i = 0; i < overlayIcons.Count; i++)
            {
                if (i == 0)
                    CombineTextures(finalIcon, texture, overlayIcons[i]);
                else
                    CombineTextures(finalIcon, finalIcon, overlayIcons[i]);
            }

            return finalIcon;
        }

        public static void CombineTextures(Texture2D final, Texture2D image, Texture2D overlay)
        {
            Vector2Int offset = new Vector2Int(Mathf.FloorToInt((final.width - overlay.width) * 0.5f), Mathf.FloorToInt((final.height - overlay.height) * 0.5f));

            final.SetPixels(image.GetPixels());

            for (int y = 0; y < overlay.height; y++)
            {
                for (int x = 0; x < overlay.width; x++)
                {
                    Color PixelColorFront = overlay.GetPixel(x, y) * overlay.GetPixel(x, y).a;
                    Color PixelColorBack = final.GetPixel(x + offset.x, y + offset.y) * (1 - PixelColorFront.a);
                    final.SetPixel(x + offset.x, y + offset.y, PixelColorBack + PixelColorFront);
                }
            }

            final.Apply();
        }

        public static void ReplaceColor(Texture2D texture, Color colorToReplace, Color newColor)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    if (pixelColor == colorToReplace)
                    //if (Mathf.Approximately(pixelColor.r, colorToReplace.r) && Mathf.Approximately(pixelColor.g, colorToReplace.g) && Mathf.Approximately(pixelColor.b, colorToReplace.b))
                    {
                        texture.SetPixel(x, y, newColor);
                    }
                }
            }

            texture.Apply();
        }
    }
}