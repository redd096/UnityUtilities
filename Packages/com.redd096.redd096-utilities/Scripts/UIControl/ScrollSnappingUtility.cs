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
        /// This will add an EventTrigger to your ScrollRect and will register to OnBeginDrag and OnEndDrag, to stop and start snapping
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="delayBeforeSnap">Delay after user EndDrag before start Snapping</param>
        /// <param name="snapSpeed"></param>
        /// <param name="onBeginDrag">When user begin drag the scrollrect</param>
        /// <param name="onEndDrag">When user stop drag the scrollrect (before start snapping)</param>
        /// <param name="onStartSnap">When start the snapping coroutine (after delayBeforeSnap)</param>
        /// <param name="onUpdateSnap">While snapping (it is called only if child is NOT null)</param>
        /// <param name="onCompleteSnap">When the snapping coroutine is completed</param>
        public static void SetSnapping(
            ScrollRect scrollRect,
            float delayBeforeSnap = 0f,
            float snapSpeed = 10f,
            System.Action<ScrollRect> onBeginDrag = null,
            System.Action<ScrollRect> onEndDrag = null,
            System.Action<ScrollRect, Transform> onStartSnap = null,
            System.Action<ScrollRect, Transform> onUpdateSnap = null,
            System.Action<ScrollRect, Transform> onCompleteSnap = null)
        {
            SetSnapping(scrollRect, out _, out _, out _, delayBeforeSnap, snapSpeed, onBeginDrag, onEndDrag, onStartSnap, onUpdateSnap, onCompleteSnap);
        }

        /// <summary>
        /// This will add an EventTrigger to your ScrollRect and will register to OnBeginDrag and OnEndDrag, to stop and start snapping
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="eventTrigger">Added EventTrigger</param>
        /// <param name="beginDragEntry">EventTrigger added to reigster to OnBeginDrag</param>
        /// <param name="endDragEntry">EventTrigger added to reigster to OnEndDrag</param>
        /// <param name="delayBeforeSnap">Delay after user EndDrag before start Snapping</param>
        /// <param name="snapSpeed"></param>
        /// <param name="onBeginDrag">When user begin drag the scrollrect</param>
        /// <param name="onEndDrag">When user stop drag the scrollrect (before start snapping)</param>
        /// <param name="onStartSnap">When start the snapping coroutine (after delayBeforeSnap)</param>
        /// <param name="onUpdateSnap">While snapping (it is called only if child is NOT null)</param>
        /// <param name="onCompleteSnap">When the snapping coroutine is completed</param>
        public static void SetSnapping(
            ScrollRect scrollRect,
            out EventTrigger eventTrigger,
            out EventTrigger.Entry beginDragEntry,
            out EventTrigger.Entry endDragEntry,
            float delayBeforeSnap = 0f,
            float snapSpeed = 10f,
            System.Action<ScrollRect> onBeginDrag = null,
            System.Action<ScrollRect> onEndDrag = null,
            System.Action<ScrollRect, Transform> onStartSnap = null,
            System.Action<ScrollRect, Transform> onUpdateSnap = null,
            System.Action<ScrollRect, Transform> onCompleteSnap = null)
        {
            //add event trigger
            eventTrigger = scrollRect.gameObject.AddComponent<EventTrigger>();

            //begin drag
            beginDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDragEntry.callback.AddListener(eventData => OnBeginDrag(scrollRect, onBeginDrag));
            eventTrigger.triggers.Add(beginDragEntry);

            //end drag
            endDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDragEntry.callback.AddListener(eventData => OnEndDrag(scrollRect, delayBeforeSnap, snapSpeed, onEndDrag, onStartSnap, onUpdateSnap, onCompleteSnap));
            eventTrigger.triggers.Add(endDragEntry);
        }

        /// <summary>
        /// This will start the snapping coroutine
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="child">child to snap to</param>
        /// <param name="snapSpeed"></param>
        /// <param name="onUpdateSnap">While snapping (it is called only if child is NOT null)</param>
        /// <param name="onCompleteSnap">When the snapping coroutine is completed</param>
        public static void SnapToChild(
            ScrollRect scrollRect,
            Transform child,
            float snapSpeed = 10f,
            System.Action<ScrollRect, Transform> onUpdateSnap = null,
            System.Action<ScrollRect, Transform> onCompleteSnap = null)
        {
            //to be sure, stop coroutine
            if (snapCoroutines.ContainsKey(scrollRect))
            {
                scrollRect.StopCoroutine(snapCoroutines[scrollRect]);
                snapCoroutines.Remove(scrollRect);
            }

            //start coroutine
            snapCoroutines.Add(scrollRect, scrollRect.StartCoroutine(SnapToChildCoroutine(scrollRect, child, snapSpeed, onUpdateSnap, onCompleteSnap)));
        }

        #region private API

        private static void OnBeginDrag(
            ScrollRect scrollRect,
            System.Action<ScrollRect> onBeginDrag)
        {
            //stop coroutine OnBegin Drag
            if (snapCoroutines.ContainsKey(scrollRect))
            {
                scrollRect.StopCoroutine(snapCoroutines[scrollRect]);
                snapCoroutines.Remove(scrollRect);
            }

            onBeginDrag?.Invoke(scrollRect);
        }

        private static void OnEndDrag(
            ScrollRect scrollRect,
            float delayBeforeSnap,
            float snapSpeed,
            System.Action<ScrollRect> onEndDrag,
            System.Action<ScrollRect, Transform> onStartSnap,
            System.Action<ScrollRect, Transform> onUpdateSnap,
            System.Action<ScrollRect, Transform> onCompleteSnap)
        {
            //just to be sure, stop coroutine
            if (snapCoroutines.ContainsKey(scrollRect))
            {
                scrollRect.StopCoroutine(snapCoroutines[scrollRect]);
                snapCoroutines.Remove(scrollRect);
            }

            onEndDrag?.Invoke(scrollRect);

            //start coroutine OnEnd Drag
            snapCoroutines.Add(scrollRect, scrollRect.StartCoroutine(SnapCoroutine(scrollRect, delayBeforeSnap, snapSpeed, onStartSnap, onUpdateSnap, onCompleteSnap)));
        }

        private static IEnumerator SnapCoroutine(
            ScrollRect scrollRect,
            float delayBeforeSnap,
            float snapSpeed,
            System.Action<ScrollRect, Transform> onStartSnap,
            System.Action<ScrollRect, Transform> onUpdateSnap,
            System.Action<ScrollRect, Transform> onCompleteSnap)
        {
            //if user doesn't scroll for few seconds, start snapping
            yield return new WaitForSeconds(delayBeforeSnap);

            Transform container = scrollRect.content;
            RectTransform scrollRt = scrollRect.GetComponent<RectTransform>();
            Vector3 center = scrollRt.TransformPoint(scrollRt.rect.center);

            //find nearest child
            float closestDistance = float.MaxValue;
            Transform closestChild = null;
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

            //and snap to it
            onStartSnap?.Invoke(scrollRect, closestChild);
            yield return SnapToChildCoroutine(scrollRect, closestChild, snapSpeed, onUpdateSnap, onCompleteSnap);
        }

        private static IEnumerator SnapToChildCoroutine(
            ScrollRect scrollRect,
            Transform child,
            float snapSpeed,
            System.Action<ScrollRect, Transform> onUpdateSnap,
            System.Action<ScrollRect, Transform> onCompleteSnap)
        {
            //calculate center
            Transform container = scrollRect.content;
            RectTransform scrollRt = scrollRect.GetComponent<RectTransform>();
            Vector3 center = scrollRt.TransformPoint(scrollRt.rect.center);

            if (child)
            {
                //calculate distance
                Vector3 targetPosition = container.localPosition;
                Vector3 difference = container.InverseTransformPoint(child.position) - container.InverseTransformPoint(center);
                targetPosition.x -= difference.x;

                //do snapping
                while (Vector3.Distance(container.localPosition, targetPosition) > 0.1f)
                {
                    container.localPosition = Vector3.Lerp(container.localPosition, targetPosition, Time.deltaTime * snapSpeed);
                    onUpdateSnap?.Invoke(scrollRect, child);
                    yield return null;
                }

                container.localPosition = targetPosition;
            }

            onCompleteSnap?.Invoke(scrollRect, child);
        }

        #endregion
    }
}