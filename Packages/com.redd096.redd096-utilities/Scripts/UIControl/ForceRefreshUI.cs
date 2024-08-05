using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.UIControl
{
    [AddComponentMenu("redd096/UIControl/Force Refresh UI")]
    public class ForceRefreshUI : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(RefreshLayoutGroupsImmediateAndRecursive(gameObject));
        }

        /// <summary>
        /// Wait end of frame and call ForceRebuildLayoutImmediate on every child layout group
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerator RefreshLayoutGroupsImmediateAndRecursive(GameObject root)
        {
            //wait end of frame
            yield return new WaitForEndOfFrame();

            if (root)
            {
                //refresh every child layout group
                var componentsInChildren = root.GetComponentsInChildren<LayoutGroup>(true);
                foreach (var layoutGroup in componentsInChildren)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
                }

                //be sure root is refreshed as last one
                var rootRect = root.GetComponent<RectTransform>();
                if (rootRect)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rootRect);
            }
        }
    }
}