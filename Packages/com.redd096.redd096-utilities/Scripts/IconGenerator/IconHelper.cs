using System.Collections.Generic;
using UnityEngine;

namespace redd096.IconGenerator
{
    public static class IconHelper
    {
        /// <summary>
        /// Add backgrounds behind texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="backgroundIcons"></param>
        /// <returns></returns>
        public static Texture2D ApplyBackgroundsToTexture(Texture2D texture, List<Texture2D> backgroundIcons)
        {
            //if there aren't backgrounds, return texture
            if (backgroundIcons == null || backgroundIcons.Count == 0)
                return texture;

            //else generate new texture
            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            //and combine with background
            if (backgroundIcons.Count == 1)
            {
                CombineTextures(backgroundIcons[0], texture, result);
            }
            else if (backgroundIcons.Count > 1)
            {
                //if there are more backgrounds, combine all them
                CombineTextures(backgroundIcons[0], backgroundIcons[1], result);
                for (int i = 2; i < backgroundIcons.Count; i++)
                {
                    CombineTextures(result, backgroundIcons[i], result);
                }
                //then combine result with texture
                CombineTextures(result, texture, result);
            }

            return result;
        }

        /// <summary>
        /// Add overlays to texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="overlayIcons"></param>
        /// <returns></returns>
        public static Texture2D ApplyOverlaysToTexture(Texture2D texture, List<Texture2D> overlayIcons)
        {
            //if there aren't overlays, return texture
            if (overlayIcons == null || overlayIcons.Count == 0)
                return texture;

            //else generate new texture
            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            //and combine with overlays
            CombineTextures(texture, overlayIcons[0], result);
            for (int i = 1; i < overlayIcons.Count; i++)
            {
                CombineTextures(result, overlayIcons[i], result);
            }

            return result;
        }

        /// <summary>
        /// Add overlay to texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="overlay"></param>
        /// <param name="result"></param>
        private static void CombineTextures(Texture2D texture, Texture2D overlay, Texture2D result)
        {
            Vector2Int offset = new Vector2Int(Mathf.FloorToInt((result.width - overlay.width) * 0.5f), Mathf.FloorToInt((result.height - overlay.height) * 0.5f));

            result.SetPixels(texture.GetPixels());

            for (int y = 0; y < overlay.height; y++)
            {
                for (int x = 0; x < overlay.width; x++)
                {
                    Color PixelColorFront = overlay.GetPixel(x, y) * overlay.GetPixel(x, y).a;
                    Color PixelColorBack = result.GetPixel(x + offset.x, y + offset.y) * (1 - PixelColorFront.a);
                    result.SetPixel(x + offset.x, y + offset.y, PixelColorBack + PixelColorFront);
                }
            }

            result.Apply();
        }

        /// <summary>
        /// Replace pixels color in texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="colorToReplace"></param>
        /// <param name="newColor"></param>
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