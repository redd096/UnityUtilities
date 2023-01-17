using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace redd096
{
    [AddComponentMenu("redd096/Singletons/Utility Singleton")]
    public class CursorHandOver : Singleton<CursorHandOver>
    {
        [Header("if null, use default cursor")]
        [SerializeField] Texture2D defaultIcon = default;
        [SerializeField] Vector2 defaultOffset = Vector2.zero;
        [SerializeField] CursorMode defaultMode = CursorMode.Auto;
        [Space(10)]
        [SerializeField] Texture2D handIcon = default;
        [SerializeField] Vector2 handOffset = Vector2.zero;
        [SerializeField] CursorMode handMode = CursorMode.ForceSoftware;

        private bool isHand;

        private void Update()
        {
            //if pointer is over UI object
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //check if hit a Selectable
                if (HasThisComponentFirstHit(typeof(Selectable), typeof(IPointerClickHandler), typeof(IPointerDownHandler)))
                {
                    if (isHand == false)
                    {
                        isHand = true;
                        Cursor.SetCursor(handIcon, handOffset, handMode);
                    }
                    return;
                }
            }

            //else use default cursor
            if (isHand)
            {
                isHand = false;
                Cursor.SetCursor(defaultIcon, defaultOffset, defaultMode);
            }
        }

        #region raycast UI

        List<RaycastResult> GetHitsAtPosition()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
                return new List<RaycastResult>();

            //set pointer event at input position
            PointerEventData pointerEvent = new PointerEventData(eventSystem);
            pointerEvent.position = GetScreenInputPosition();

            //then raycast and get hits
            List<RaycastResult> hits = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEvent, hits);

            return hits;
        }

        bool HasThisComponentFirstHit(params System.Type[] types)
        {
            List<RaycastResult> hits = GetHitsAtPosition();

            //if hit something, get first hit
            if (hits != null && hits.Count > 0)
            {
                //check every passed type
                foreach (System.Type type in types)
                {
                    if (hits[0].gameObject.GetComponentInParent(type))
                        return true;
                }
            }

            return false;
        }

        Vector2 GetScreenInputPosition()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Input.GetTouch(0).position;
#else
            return Input.mousePosition;
#endif
        }

        #endregion
    }
}