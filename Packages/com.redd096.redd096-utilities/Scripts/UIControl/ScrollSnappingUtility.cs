using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace redd096.UIControl
{
    public static class ScrollSnappingUtility
    {
        private static Dictionary<ScrollRect, Coroutine> snapCoroutines = new Dictionary<ScrollRect, Coroutine>();

        /// <summary>
        /// This will add an EventTrigger to your ScrollRect and will call OnBeginDrag and OnEndDrag
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="delayBeforeSnap">Delay after user EndDrag before start Snapping</param>
        /// <param name="snapSpeed"></param>
        public static void SetSnapping(ScrollRect scrollRect, float delayBeforeSnap = 0f, float snapSpeed = 10f)
        {
            //add event trigger
            EventTrigger eventTrigger = scrollRect.gameObject.AddComponent<EventTrigger>();

            //begin drag
            EventTrigger.Entry beginDrag = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDrag.callback.AddListener(eventData => OnBeginDrag(scrollRect));
            eventTrigger.triggers.Add(beginDrag);

            //end drag
            EventTrigger.Entry endDrag = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDrag.callback.AddListener(eventData => OnEndDrag(scrollRect, delayBeforeSnap, snapSpeed));
            eventTrigger.triggers.Add(endDrag);
        }

        private static void OnBeginDrag(ScrollRect scrollRect)
        {
            //stop coroutine OnBegin Drag
            if (snapCoroutines.ContainsKey(scrollRect))
            {
                scrollRect.StopCoroutine(snapCoroutines[scrollRect]);
                snapCoroutines.Remove(scrollRect);
            }
        }

        private static void OnEndDrag(ScrollRect scrollRect, float delayBeforeSnap, float snapSpeed)
        {
            //just to be sure, stop coroutine
            if (snapCoroutines.ContainsKey(scrollRect))
            {
                scrollRect.StopCoroutine(snapCoroutines[scrollRect]);
                snapCoroutines.Remove(scrollRect);
            }

            //start coroutine OnEnd Drag
            snapCoroutines.Add(scrollRect, scrollRect.StartCoroutine(SnapCoroutine(scrollRect, delayBeforeSnap, snapSpeed)));
        }

        private static IEnumerator SnapCoroutine(ScrollRect scrollRect, float delayBeforeSnap, float snapSpeed)
        {
            //if user doesn't scroll for few seconds, start snapping
            yield return new WaitForSeconds(delayBeforeSnap);

            Transform container = scrollRect.content;
            RectTransform scrollRt = scrollRect.GetComponent<RectTransform>();
            Vector3 center = scrollRt.TransformPoint(scrollRt.rect.center);

            //find nearest child
            float closestDistance = float.MaxValue;
            RectTransform closestChild = null;
            foreach (RectTransform child in container)
            {
                Vector3 childCenter = child.TransformPoint(child.rect.center);
                float distance = Mathf.Abs(childCenter.x - center.x);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestChild = child;
                }
            }

            if (closestChild)
            {
                //calculate distance
                Vector3 targetPosition = container.localPosition;
                Vector3 difference = container.InverseTransformPoint(closestChild.position) - container.InverseTransformPoint(center);
                targetPosition.x -= difference.x;

                //do snapping
                while (Vector3.Distance(container.localPosition, targetPosition) > 0.1f)
                {
                    container.localPosition = Vector3.Lerp(container.localPosition, targetPosition, Time.deltaTime * snapSpeed);
                    yield return null;
                }
                container.localPosition = targetPosition;
            }
        }
    }
}