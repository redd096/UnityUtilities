using System.Collections;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Main/MonoBehaviours/Screenshot Utility")]
    public class ScreenshotUtility : MonoBehaviour
    {
        [SerializeField] Camera screenshotCamera;

        public static Vector2Int ScreenResolution => new Vector2Int(Screen.width, Screen.height);
        public static Vector2Int Resolution4K => new Vector2Int(3840, 2160);

        /// <summary>
        /// Generate a Texture2D with Camera.Render()
        /// </summary>
        /// <param name="textureResolution">Resolution of the Texture2D. e.g. in this script there are static vars ScreenResolution and Resolution4K</param>
        public Texture2D DoScreenshot(Vector2Int textureResolution) => DoScreenshot(screenshotCamera, textureResolution);
        /// <summary>
        /// Generate a Texture2D with Camera.Render(). Texture resolution depend from screen resolution
        /// </summary>
        public Texture2D DoScreenshotScreenResolution() => DoScreenshot(screenshotCamera, ScreenResolution);
        /// <summary>
        /// Generate a Texture2D with Camera.Render(). Texture resolution will be in 4K
        /// </summary>
        public Texture2D DoScreenshot4K() => DoScreenshot(screenshotCamera, Resolution4K);

        /// <summary>
        /// Generate a Texture2D with Camera.Render()
        /// </summary>
        /// <param name="cam">Camera to use for the rendering</param>
        /// <param name="textureResolution">Resolution of the Texture2D. e.g. in this script there are static vars ScreenResolution and Resolution4K</param>
        /// <returns></returns>
        public static Texture2D DoScreenshot(Camera cam, Vector2Int textureResolution)
        {
            // Save values
            var oldActiveRenderTexture = RenderTexture.active;
            var oldCameraTargetTexture = cam.targetTexture;

            // Create a new RenderTexture with the necessary dimensions    
            RenderTexture renderTexture = new RenderTexture(textureResolution.x, textureResolution.y, 24);

            // Use Camera to render on RenderTexture
            cam.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            cam.Render();

            // Create result Texture
            Texture2D resultTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            resultTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            resultTexture.Apply();

            // Reset values
            RenderTexture.active = oldActiveRenderTexture;
            cam.targetTexture = oldCameraTargetTexture;

            // Clear memory
            Object.Destroy(renderTexture);
            //Object.Destroy(resultTexture);

            return resultTexture;
        }

        /// <summary>
        /// Generate a Texture2D with Camera.Render(). Wait one frame between set RenderTexture and Render on camera. This shouldn't be necessary, but you know...
        /// </summary>
        /// <param name="cam">Camera to use for the rendering</param>
        /// <param name="textureResolution">Resolution of the Texture2D. e.g. in this script there are static vars ScreenResolution and Resolution4K</param>
        /// <param name="onComplete">Callback with result texture</param>
        public static IEnumerator DoScreenshotWithDelay(Camera cam, Vector2Int textureResolution, System.Action<Texture2D> onComplete)
        {
            // Save values
            var oldActiveRenderTexture = RenderTexture.active;
            var oldCameraTargetTexture = cam.targetTexture;

            // Create a new RenderTexture with the necessary dimensions    
            RenderTexture renderTexture = new RenderTexture(textureResolution.x, textureResolution.y, 24);

            // Use Camera to render on RenderTexture
            cam.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            yield return null;  //wait one frame after set render texture
            cam.Render();

            // Create result Texture
            Texture2D resultTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            resultTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            resultTexture.Apply();

            // Reset values
            RenderTexture.active = oldActiveRenderTexture;
            cam.targetTexture = oldCameraTargetTexture;

            // Clear memory
            Object.Destroy(renderTexture);
            //Object.Destroy(resultTexture);

            onComplete?.Invoke(resultTexture);
        }
    }
}